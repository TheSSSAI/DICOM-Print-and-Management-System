// DMPS.Client.Presentation/Infrastructure/Rendering/DirectXRenderer.cs

using System;
using System.Diagnostics;
using System.Numerics;
using SharpGen.Runtime;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace DMPS.Client.Presentation.Infrastructure.Rendering
{
    /// <summary>
    /// Manages all DirectX 11 rendering logic for displaying a single DICOM image.
    /// This class encapsulates the creation of D3D devices, shaders, textures, and the rendering pipeline.
    /// It is designed to be used by a WPF control with DirectX interop (e.g., D3DImage).
    /// Implements IDisposable to ensure all native GPU resources are released properly.
    /// Fulfills REQ-1-052: Provide GPU-accelerated rendering for DICOM images.
    /// </summary>
    public sealed class DirectXRenderer : IDisposable
    {
        private ID3D11Device _device;
        private ID3D11DeviceContext _deviceContext;
        private FeatureLevel _featureLevel;

        private ID3D11VertexShader _vertexShader;
        private ID3D11PixelShader _pixelShader;
        private ID3D11InputLayout _inputLayout;

        private ID3D11Buffer _vertexBuffer;
        private ID3D11Buffer _constantBuffer;

        private ID3D11Texture2D _imageTexture;
        private ID3D11ShaderResourceView _imageShaderResourceView;
        private ID3D11SamplerState _samplerState;

        private bool _disposed;

        private const string VertexShaderSource = @"
            struct VS_INPUT
            {
                float4 Pos : POSITION;
                float2 Tex : TEXCOORD0;
            };

            struct PS_INPUT
            {
                float4 Pos : SV_POSITION;
                float2 Tex : TEXCOORD0;
            };

            PS_INPUT VS(VS_INPUT input)
            {
                PS_INPUT output = (PS_INPUT)0;
                output.Pos = input.Pos;
                output.Tex = input.Tex;
                return output;
            }
        ";

        private const string PixelShaderSource = @"
            Texture2D txDiffuse : register(t0);
            SamplerState samLinear : register(s0);

            cbuffer CBuffer : register(b0)
            {
                float windowWidth;
                float windowCenter;
            }

            struct PS_INPUT
            {
                float4 Pos : SV_POSITION;
                float2 Tex : TEXCOORD0;
            };

            float4 PS(PS_INPUT input) : SV_Target
            {
                // Sample the monochrome texture (value is in the red channel)
                float rawValue = txDiffuse.Sample(samLinear, input.Tex).r;
                
                // Apply window/level formula
                float windowMin = windowCenter - windowWidth / 2.0;
                float normalizedValue = (rawValue - windowMin) / windowWidth;
                
                // Clamp the value to the [0, 1] range
                float finalValue = saturate(normalizedValue);

                // Output as grayscale color
                return float4(finalValue, finalValue, finalValue, 1.0);
            }
        ";

        public DirectXRenderer()
        {
            InitializeDirect3D();
        }

        private void InitializeDirect3D()
        {
            var creationFlags = DeviceCreationFlags.BgraSupport;
#if DEBUG
            creationFlags |= DeviceCreationFlags.Debug;
#endif

            var result = D3D11.D3D11CreateDevice(
                null,
                DriverType.Hardware,
                creationFlags,
                new[] { FeatureLevel.Level_11_0 },
                out _device,
                out _featureLevel,
                out _deviceContext);

            if (result.Failure)
            {
                // Fallback to WARP device
                result = D3D11.D3D11CreateDevice(
                    null,
                    DriverType.Warp,
                    creationFlags,
                    new[] { FeatureLevel.Level_11_0 },
                    out _device,
                    out _featureLevel,
                    out _deviceContext);

                result.CheckError();
            }

            CreateShaders();
            CreateVertexBuffer();
            CreateConstantBuffer();
            CreateSamplerState();
        }

        private void CreateShaders()
        {
            var vertexShaderBlob = CompileShader(VertexShaderSource, "VS", "vs_4_0");
            _vertexShader = _device.CreateVertexShader(vertexShaderBlob);

            var pixelShaderBlob = CompileShader(PixelShaderSource, "PS", "ps_4_0");
            _pixelShader = _device.CreatePixelShader(pixelShaderBlob);

            var inputElements = new[]
            {
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
            };
            _inputLayout = _device.CreateInputLayout(inputElements, vertexShaderBlob);

            vertexShaderBlob.Dispose();
            pixelShaderBlob.Dispose();
        }

        private static Blob CompileShader(string source, string entryPoint, string profile)
        {
            var result = Compiler.Compile(source, entryPoint, profile, ShaderFlags.OptimizationLevel3);
            if (result.Failure)
            {
                throw new Exception($"Shader compilation failed: {result.Message}");
            }
            return result;
        }

        private void CreateVertexBuffer()
        {
            var vertices = new[]
            {
                // Full-screen quad
                new Vector3(-1.0f,  1.0f, 0.5f), new Vector2(0.0f, 0.0f), // Top-left
                new Vector3( 1.0f, -1.0f, 0.5f), new Vector2(1.0f, 1.0f), // Bottom-right
                new Vector3(-1.0f, -1.0f, 0.5f), new Vector2(0.0f, 1.0f), // Bottom-left
                
                new Vector3( 1.0f,  1.0f, 0.5f), new Vector2(1.0f, 0.0f), // Top-right
                new Vector3( 1.0f, -1.0f, 0.5f), new Vector2(1.0f, 1.0f), // Bottom-right
                new Vector3(-1.0f,  1.0f, 0.5f), new Vector2(0.0f, 0.0f), // Top-left
            };

            _vertexBuffer = _device.CreateBuffer(vertices, BindFlags.VertexBuffer);
        }

        private void CreateConstantBuffer()
        {
            var bufferDescription = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                ByteWidth = Interop.SizeOf<WindowLevelStruct>(),
                BindFlags = BindFlags.ConstantBuffer
            };
            _constantBuffer = _device.CreateBuffer(bufferDescription);
        }

        private void CreateSamplerState()
        {
            var samplerDesc = new SamplerDescription(Filter.MinMagMipLinear, TextureAddressMode.Clamp);
            _samplerState = _device.CreateSamplerState(samplerDesc);
        }

        public void UpdateImage(IntPtr pixelData, int width, int height, int stride, Format format)
        {
            DisposeImageResources();

            var textureDesc = new Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            var dataRectangle = new DataRectangle(pixelData, stride);
            _imageTexture = _device.CreateTexture2D(textureDesc, dataRectangle);
            _imageShaderResourceView = _device.CreateShaderResourceView(_imageTexture);
        }

        public void UpdateWindowLevel(float windowWidth, float windowCenter)
        {
            var data = new WindowLevelStruct { WindowWidth = windowWidth, WindowCenter = windowCenter };
            _deviceContext.UpdateSubresource(data, _constantBuffer);
        }

        public void Render(ID3D11RenderTargetView renderTargetView, int width, int height)
        {
            if (_deviceContext == null || renderTargetView == null || _imageShaderResourceView == null)
            {
                return;
            }

            _deviceContext.OMSetRenderTargets(renderTargetView);
            _deviceContext.RSSetViewport(new Viewport(0, 0, width, height, 0.0f, 1.0f));

            _deviceContext.IASetInputLayout(_inputLayout);
            _deviceContext.IASetVertexBuffer(0, _vertexBuffer, Interop.SizeOf<Vector3>() + Interop.SizeOf<Vector2>());
            _deviceContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

            _deviceContext.VSSetShader(_vertexShader);
            _deviceContext.PSSetShader(_pixelShader);
            _deviceContext.PSSetConstantBuffer(0, _constantBuffer);
            _deviceContext.PSSetShaderResource(0, _imageShaderResourceView);
            _deviceContext.PSSetSampler(0, _samplerState);

            _deviceContext.Draw(6, 0);

            _deviceContext.Flush();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            
            DisposeImageResources();
            _samplerState?.Dispose();
            _constantBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _inputLayout?.Dispose();
            _pixelShader?.Dispose();
            _vertexShader?.Dispose();
            _deviceContext?.Dispose();
#if DEBUG
            if (_device != null)
            {
                var debug = _device.QueryInterfaceOrNull<ID3D11Debug>();
                if (debug != null)
                {
                    debug.ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail);
                    debug.Dispose();
                }
            }
#endif
            _device?.Dispose();
        }

        private void DisposeImageResources()
        {
            _imageShaderResourceView?.Dispose();
            _imageTexture?.Dispose();
            _imageShaderResourceView = null;
            _imageTexture = null;
        }

        // Struct to match the layout of the constant buffer in the shader
        private struct WindowLevelStruct
        {
            public float WindowWidth;
            public float WindowCenter;
            private float _padding1; // Padding to ensure 16-byte alignment
            private float _padding2;
        }
    }
}
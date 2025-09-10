using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.D3DCompiler;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace DMPS.Client.Presentation.Infrastructure.Rendering
{
    /// <summary>
    /// Implements a DirectX 11 renderer using the Vortice.Windows library.
    /// This class encapsulates all low-level graphics operations required to render a texture (e.g., a DICOM image)
    /// onto a surface, including window/level adjustments via shaders.
    /// It is designed to be used by a WPF interop control (e.g., D3DImage).
    /// </summary>
    public sealed class DirectXRenderer : IDisposable
    {
        #region Shader Code
        private const string ShaderSource = @"
struct VS_IN
{
    float4 pos : POSITION;
    float2 tex : TEXCOORD;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;
    output.pos = input.pos;
    output.tex = input.tex;
    return output;
}

Texture2D DicomTexture : register(t0);
SamplerState Sampler    : register(s0);

cbuffer WwlParams : register(b0)
{
    float WindowWidth;
    float WindowCenter;
};

float4 PS(PS_IN input) : SV_Target
{
    // Sample the texture. Assuming single-channel (monochrome) DICOM data.
    float pixelValue = DicomTexture.Sample(Sampler, input.tex).r;
    
    // Apply window/level (WW/WL) adjustment
    float windowMin = WindowCenter - WindowWidth / 2.0f;
    float normalizedValue = (pixelValue - windowMin) / WindowWidth;
    
    // Clamp the value to the [0, 1] range
    float clampedValue = saturate(normalizedValue);
    
    // Output as a grayscale color
    return float4(clampedValue, clampedValue, clampedValue, 1.0f);
}
";
        #endregion

        #region Fields
        private ID3D11Device? _device;
        private ID3D11DeviceContext? _deviceContext;
        private IDXGISwapChain1? _swapChain;
        private ID3D11Texture2D? _renderTarget;
        private ID3D11RenderTargetView? _renderTargetView;
        private ID3D11Texture2D? _dicomTexture;
        private ID3D11ShaderResourceView? _dicomShaderResourceView;
        private ID3D11VertexShader? _vertexShader;
        private ID3D11PixelShader? _pixelShader;
        private ID3D11InputLayout? _inputLayout;
        private ID3D11Buffer? _vertexBuffer;
        private ID3D11Buffer? _constantBuffer;
        private ID3D11SamplerState? _samplerState;

        private int _width;
        private int _height;
        private bool _disposed;
        #endregion

        #region Properties
        public ID3D11Texture2D? BackBuffer => _renderTarget;
        #endregion

        public void Initialize(IntPtr windowHandle, int width, int height)
        {
            if (_device != null) return; // Already initialized

            _width = width;
            _height = height;

            CreateDeviceAndSwapChain(windowHandle);
            CreateShadersAndInputLayout();
            CreateVertexBuffer();
            CreateConstantBuffer();
            CreateSamplerState();
        }

        public void Resize(int width, int height)
        {
            if (_device == null || _deviceContext == null || _swapChain == null) return;
            if (_width == width && _height == height) return;

            _width = width;
            _height = height;

            _renderTargetView?.Dispose();
            _renderTarget?.Dispose();

            _swapChain.ResizeBuffers(2, _width, _height, Format.B8G8R8A8_UNorm, SwapChainFlags.None).CheckError();

            using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
            {
                _renderTarget = backBuffer;
                _renderTargetView = _device.CreateRenderTargetView(backBuffer);
            }

            _deviceContext.RSSetViewport(0, 0, _width, _height);
        }

        public void UpdateDicomTexture(byte[] pixelData, int textureWidth, int textureHeight, Format format)
        {
            if (_device == null)
            {
                throw new InvalidOperationException("Renderer is not initialized.");
            }

            // Dispose previous texture resources if they exist
            _dicomShaderResourceView?.Dispose();
            _dicomTexture?.Dispose();

            var textureDescription = new Texture2DDescription
            {
                Width = textureWidth,
                Height = textureHeight,
                MipLevels = 1,
                ArraySize = 1,
                Format = format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            _dicomTexture = _device.CreateTexture2D(textureDescription, new Span<byte>(pixelData));
            _dicomShaderResourceView = _device.CreateShaderResourceView(_dicomTexture);
        }

        public void UpdateWwlParameters(float windowWidth, float windowCenter)
        {
            if (_deviceContext == null || _constantBuffer == null) return;

            var wwlData = new Vector2(windowWidth, windowCenter);
            _deviceContext.UpdateSubresource(wwlData, _constantBuffer);
        }

        public void Render()
        {
            if (_deviceContext == null || _renderTargetView == null || _swapChain == null) return;

            var clearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            _deviceContext.ClearRenderTargetView(_renderTargetView, clearColor);

            _deviceContext.OMSetRenderTargets(_renderTargetView);

            if (_vertexBuffer != null && _inputLayout != null && _vertexShader != null && _pixelShader != null && _samplerState != null && _dicomShaderResourceView != null && _constantBuffer != null)
            {
                _deviceContext.IASetVertexBuffer(0, _vertexBuffer, Unsafe.SizeOf<Vertex>());
                _deviceContext.IASetInputLayout(_inputLayout);
                _deviceContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleStrip);
                
                _deviceContext.VSSetShader(_vertexShader);
                _deviceContext.PSSetShader(_pixelShader);
                
                _deviceContext.PSSetShaderResource(0, _dicomShaderResourceView);
                _deviceContext.PSSetSampler(0, _samplerState);
                _deviceContext.PSSetConstantBuffer(0, _constantBuffer);
                
                _deviceContext.Draw(4, 0);
            }

            _swapChain.Present(1, PresentFlags.None);
        }

        #region Private Initialization Methods

        private void CreateDeviceAndSwapChain(IntPtr windowHandle)
        {
            var creationFlags = DeviceCreationFlags.BgraSupport;
#if DEBUG
            creationFlags |= DeviceCreationFlags.Debug;
#endif

            if (D3D11.D3D11CreateDevice(
                null,
                DriverType.Hardware,
                creationFlags,
                new[] { FeatureLevel.Level_11_0 },
                out _device,
                out _deviceContext).Failure)
            {
                // Fallback to WARP device
                D3D11.D3D11CreateDevice(
                    null,
                    DriverType.Warp,
                    creationFlags,
                    new[] { FeatureLevel.Level_11_0 },
                    out _device,
                    out _deviceContext).CheckError();
            }

            using (var dxgiDevice = _device.QueryInterface<IDXGIDevice>())
            using (var dxgiAdapter = dxgiDevice.GetAdapter())
            using (var dxgiFactory = dxgiAdapter.GetParent<IDXGIFactory2>())
            {
                var swapChainDesc = new SwapChainDescription1
                {
                    Width = _width,
                    Height = _height,
                    Format = Format.B8G8R8A8_UNorm,
                    Stereo = false,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = Usage.RenderTargetOutput,
                    BufferCount = 2,
                    Scaling = Scaling.Stretch,
                    SwapEffect = SwapEffect.FlipSequential,
                    AlphaMode = AlphaMode.Premultiplied,
                    Flags = SwapChainFlags.None
                };

                _swapChain = dxgiFactory.CreateSwapChainForHwnd(_device, windowHandle, swapChainDesc);
            }
            
            Resize(_width, _height);
        }

        private void CreateShadersAndInputLayout()
        {
            if (_device == null) throw new InvalidOperationException();

            using var vertexShaderBlob = CompileShader(ShaderSource, "VS", "vs_5_0");
            _vertexShader = _device.CreateVertexShader(vertexShaderBlob);

            using var pixelShaderBlob = CompileShader(ShaderSource, "PS", "ps_5_0");
            _pixelShader = _device.CreatePixelShader(pixelShaderBlob);

            var inputElements = new[]
            {
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0)
            };
            _inputLayout = _device.CreateInputLayout(inputElements, vertexShaderBlob);
        }

        private void CreateVertexBuffer()
        {
            if (_device == null) throw new InvalidOperationException();

            var vertices = new[]
            {
                new Vertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(0.0f, 0.0f)),  // Top-left
                new Vertex(new Vector3(1.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f)),   // Top-right
                new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), new Vector2(0.0f, 1.0f)), // Bottom-left
                new Vertex(new Vector3(1.0f, -1.0f, 0.0f), new Vector2(1.0f, 1.0f))  // Bottom-right
            };

            _vertexBuffer = _device.CreateBuffer(vertices, BindFlags.VertexBuffer);
        }

        private void CreateConstantBuffer()
        {
            if (_device == null) throw new InvalidOperationException();
            
            var bufferDesc = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                ByteWidth = Unsafe.SizeOf<Vector2>(),
                BindFlags = BindFlags.ConstantBuffer
            };
            _constantBuffer = _device.CreateBuffer(bufferDesc);
        }

        private void CreateSamplerState()
        {
            if (_device == null) throw new InvalidOperationException();

            var samplerDesc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunction = ComparisonFunction.Never,
                MinLOD = 0,
                MaxLOD = float.MaxValue
            };
            _samplerState = _device.CreateSamplerState(samplerDesc);
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

        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Release resources in reverse order of creation
            _samplerState?.Dispose();
            _constantBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _inputLayout?.Dispose();
            _pixelShader?.Dispose();
            _vertexShader?.Dispose();
            _dicomShaderResourceView?.Dispose();
            _dicomTexture?.Dispose();
            _renderTargetView?.Dispose();
            _renderTarget?.Dispose();
            _swapChain?.Dispose();
            _deviceContext?.Dispose();
            
#if DEBUG
            if (_device != null)
            {
                var debug = _device.QueryInterface<ID3D11Debug>();
                debug.ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail);
                debug.Dispose();
            }
#endif
            _device?.Dispose();

            GC.SuppressFinalize(this);
        }

        ~DirectXRenderer()
        {
            Dispose();
        }
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct Vertex
        {
            public readonly Vector3 Position;
            public readonly Vector2 TexCoord;

            public Vertex(Vector3 position, Vector2 texCoord)
            {
                Position = position;
                TexCoord = texCoord;
            }
        }
        #endregion
    }
}
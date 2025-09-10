using DMPS.Application.Interfaces;
using DMPS.Domain.Models;
using DMPS.Infrastructure.IO.Interfaces;
using DMPS.Infrastructure.IO.Pdf.Exceptions;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.IO.Pdf
{
    /// <summary>
    /// A service for generating PDF documents using the QuestPDF library.
    /// This class acts as an adapter, translating a domain-specific layout definition
    /// into a QuestPDF document structure.
    /// </summary>
    public sealed class PdfGeneratorService(ILogger<PdfGeneratorService> logger) : IPdfGenerator
    {
        /// <inheritdoc />
        public async Task<byte[]> GeneratePdfAsync(PrintLayoutDefinition layout, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(layout);

            logger.LogInformation("Starting PDF generation for layout: {LayoutName}", layout.LayoutName);

            try
            {
                var document = Document.Create(container => ComposeDocument(container, layout));
                
                // Configure PDF/A-3b compliance as per REQ-1-029
                document.WithMetadata(x =>
                {
                    x.WithPdfA(PdfAVersion.PdfA3b);
                    x.WithTitle(layout.DocumentTitle ?? "DICOM Print Export");
                    x.WithAuthor("DMPS Application");
                    x.WithSubject($"Study: {layout.StudyInstanceUid}");
                    x.WithCreationDate(DateTime.UtcNow);
                });

                logger.LogDebug("QuestPDF document model created. Starting binary generation.");
                byte[] pdfBytes = await document.GeneratePdfAsync(cancellationToken);
                logger.LogInformation("Successfully generated {PdfSizeInKb} KB PDF document.", pdfBytes.Length / 1024);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during PDF generation for layout {LayoutName}.", layout.LayoutName);
                throw new PdfGenerationException($"Failed to generate PDF for layout '{layout.LayoutName}'. See inner exception for details.", ex);
            }
        }

        private void ComposeDocument(IDocumentContainer container, PrintLayoutDefinition layout)
        {
            container.Page(page =>
            {
                // Page Setup
                page.Size(layout.IsLandscape ? PageSizes.A4.Landscape() : PageSizes.A4.Portrait());
                page.MarginVertical(layout.MarginMillimeters, Unit.Millimetre);
                page.MarginHorizontal(layout.MarginMillimeters, Unit.Millimetre);

                page.DefaultTextStyle(style => style.FontSize(10).FontFamily(Fonts.Helvetica));
                
                // Header
                page.Header().Element(c => ComposeHeader(c, layout));

                // Content
                page.Content().Element(c => ComposeContent(c, layout));

                // Footer
                page.Footer().Element(c => ComposeFooter(c, layout));
            });
        }
        
        private void ComposeHeader(IContainer container, PrintLayoutDefinition layout)
        {
            if (layout.HeaderContent is null && layout.Branding is null)
            {
                return;
            }

            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Branding/Logo
                    if (layout.Branding?.LogoBytes is { Length: > 0 })
                    {
                        row.RelativeItem(0.2f).AlignLeft().Image(layout.Branding.LogoBytes).FitArea();
                    }

                    row.ConstantItem(10);
                    
                    // Custom Text Fields with Dynamic Variables
                    if (layout.HeaderContent is not null)
                    {
                        row.RelativeItem().Column(headerCol =>
                        {
                            headerCol.Item().AlignRight().Text(text =>
                            {
                                foreach (var field in layout.HeaderContent)
                                {
                                    var content = ReplaceDynamicVariables(field.Content, layout);
                                    text.Span(content)
                                        .FontSize(field.FontSize)
                                        .FontColor(field.FontColor)
                                        .Bold(field.IsBold);
                                    text.EmptyLine();
                                }
                            });
                        });
                    }
                });

                column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten-2);
            });
        }
        
        private void ComposeContent(IContainer container, PrintLayoutDefinition layout)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                // This is a simplified grid implementation. A real-world scenario would require
                // more complex logic to handle various grid types (e.g., '1+3', '2x2', etc.).
                int imagesPerPage = layout.GridRows * layout.GridColumns;
                int currentImageIndex = 0;

                for (int i = 0; i < layout.GridRows; i++)
                {
                    column.Item().Flexible(1).Row(row =>
                    {
                        for (int j = 0; j < layout.GridColumns; j++)
                        {
                            if (currentImageIndex < layout.Images.Count)
                            {
                                var imageInfo = layout.Images[currentImageIndex];
                                row.RelativeItem(1).Element(c => ComposeImageCell(c, imageInfo, layout));
                                currentImageIndex++;
                            }
                            else
                            {
                                // Empty cell if no more images
                                row.RelativeItem(1).Border(1).BorderColor(Colors.Grey.Lighten-2);
                            }
                        }
                    });
                }
            });
        }
        
        private void ComposeImageCell(IContainer container, PrintImageInfo image, PrintLayoutDefinition layout)
        {
            container.Padding(5).Column(column =>
            {
                // Image
                column.Item().Flexible().AlignCenter().AlignMiddle().Image(image.ImageBytes)
                    .FitArea();
                
                // Image Overlays (Annotations, Patient Info)
                // This would be drawn on top of the image container.
                if(image.Overlays.Any())
                {
                    // Placeholder for overlay rendering logic
                }
                
                // Footer text for the image cell
                column.Item().PaddingTop(2).Text(text =>
                {
                    var patientName = ReplaceDynamicVariables("[PatientName]", layout);
                    text.Span($"{patientName} - Img: {image.InstanceNumber}");
                }).FontSize(8);
            });
        }

        private void ComposeFooter(IContainer container, PrintLayoutDefinition layout)
        {
            container.AlignBottom().Column(column =>
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten-2);
                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Generated on: ").FontSize(8);
                        text.CurrentDate("yyyy-MM-dd HH:mm:ss UTC");
                    });
                    
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Page ").FontSize(8);

                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });
        }

        private string ReplaceDynamicVariables(string content, PrintLayoutDefinition layout)
        {
            return content
                .Replace("[PatientID]", layout.PatientId ?? "N/A")
                .Replace("[PatientName]", layout.PatientName ?? "N/A")
                .Replace("[StudyDate]", layout.StudyDate?.ToString("yyyy-MM-dd") ?? "N/A")
                .Replace("[StudyDescription]", layout.StudyDescription ?? "N/A");
        }
    }
}
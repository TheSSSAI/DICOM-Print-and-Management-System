using DMPS.Application.Interfaces;
using DMPS.Domain.Models;
using DMPS.Infrastructure.IO.Interfaces;
using DMPS.Infrastructure.IO.Printing.Exceptions;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace DMPS.Infrastructure.IO.Printing
{
    /// <summary>
    /// A service to interact with the native Windows Print API for spooling physical print jobs.
    /// This implementation is platform-specific and requires a Windows environment.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class WindowsPrintService(ILogger<WindowsPrintService> logger) : IPrintSpooler
    {
        // State for the current print job being processed by this instance.
        // This assumes a Scoped or Transient lifetime for the service instance.
        private PrintJobData? _currentJob;
        private int _currentImageIndex;

        /// <inheritdoc />
        public void SpoolJob(PrintJobData job)
        {
            ArgumentNullException.ThrowIfNull(job);
            
            logger.LogInformation("Attempting to spool print job to printer: {PrinterName}", job.PrinterName);

            try
            {
                ValidatePrinterExists(job.PrinterName);

                _currentJob = job;
                _currentImageIndex = 0;

                using var printDocument = new PrintDocument();
                
                // Configure printer and page settings
                printDocument.PrinterSettings.PrinterName = job.PrinterName;
                printDocument.DocumentName = job.DocumentTitle ?? $"DICOM Print - {job.StudyInstanceUid}";
                
                printDocument.DefaultPageSettings.Landscape = job.IsLandscape;
                // Margins are in hundredths of an inch. Convert from mm.
                int margin = (int)(job.MarginMillimeters * 100 / 25.4);
                printDocument.DefaultPageSettings.Margins = new Margins(margin, margin, margin, margin);

                // Subscribe to the PrintPage event
                printDocument.PrintPage += PrintPageHandler;
                
                // Send the document to the print spooler. This is a blocking call.
                printDocument.Print();

                logger.LogInformation("Successfully spooled print job for {ImageCount} images to {PrinterName}", job.Images.Count, job.PrinterName);
            }
            catch (InvalidPrinterException ex)
            {
                logger.LogError(ex, "The specified printer '{PrinterName}' is invalid or not found.", job.PrinterName);
                throw new PrintSpoolingException($"The specified printer '{job.PrinterName}' is invalid.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while spooling a job to printer {PrinterName}", job.PrinterName);
                throw new PrintSpoolingException($"An unexpected error occurred while spooling the print job. See inner exception for details.", ex);
            }
            finally
            {
                // Clean up state
                _currentJob = null;
                _currentImageIndex = 0;
            }
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            if (_currentJob is null || e.Graphics is null) return;
            
            logger.LogDebug("Printing page. Starting with image index {CurrentImageIndex}.", _currentImageIndex);

            // This is a simplified implementation that prints one image per page.
            // A more complex implementation would use the GridRows/GridColumns from the job
            // data to draw a grid of images on a single page.
            
            if (_currentImageIndex < _currentJob.Images.Count)
            {
                var imageInfo = _currentJob.Images[_currentImageIndex];
                
                try
                {
                    using var imageStream = new MemoryStream(imageInfo.ImageBytes);
                    using var image = Image.FromStream(imageStream);
                    
                    // Use printable area for drawing
                    Rectangle printableArea = e.MarginBounds;
                    
                    // Simple fit-to-page logic
                    e.Graphics.DrawImage(image, printableArea);
                    
                    // Example of drawing overlay text
                    using var font = new Font("Arial", 10);
                    string patientInfo = $"Patient: {_currentJob.PatientName ?? "N/A"} - Study: {_currentJob.StudyInstanceUid}";
                    e.Graphics.DrawString(patientInfo, font, Brushes.Black, new PointF(printableArea.Left, printableArea.Bottom + 5));

                    _currentImageIndex++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to draw image with instance number {InstanceNumber} to print page.", imageInfo.InstanceNumber);
                    // Optionally, draw an error placeholder on the page
                    e.Graphics.DrawString("Error rendering image", new Font("Arial", 12), Brushes.Red, e.MarginBounds.Location);
                    _currentImageIndex++;
                }
            }

            // Check if there are more pages to print
            e.HasMorePages = _currentImageIndex < _currentJob.Images.Count;
            
            logger.LogDebug("Finished printing page. HasMorePages = {HasMorePages}", e.HasMorePages);
        }

        private void ValidatePrinterExists(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
            {
                throw new InvalidPrinterException(new PrinterSettings());
            }

            if (!PrinterSettings.InstalledPrinters.Cast<string>().Any(p => 
                string.Equals(p, printerName, StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogWarning("Printer '{PrinterName}' not found in the list of installed printers.", printerName);
                throw new InvalidPrinterException(new PrinterSettings { PrinterName = printerName });
            }
        }
    }
}
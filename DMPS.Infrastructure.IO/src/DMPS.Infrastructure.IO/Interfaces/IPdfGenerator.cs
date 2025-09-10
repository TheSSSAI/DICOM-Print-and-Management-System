using DMPS.Shared.Core.Models;
using DMPS.Infrastructure.IO.Pdf.Exceptions;
using System.Threading.Tasks;

namespace DMPS.Infrastructure.IO.Interfaces;

/// <summary>
/// Defines the contract for a service that generates PDF documents.
/// This service abstracts the underlying PDF generation library (e.g., QuestPDF).
/// </summary>
public interface IPdfGenerator
{
    /// <summary>
    /// Asynchronously generates a PDF document in memory based on a layout definition.
    /// </summary>
    /// <param name="layout">The data model defining the content and structure of the PDF to be generated.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a byte array of the generated PDF document.</returns>
    /// <exception cref="PdfGenerationException">Thrown if the PDF generation process fails for any reason.</exception>
    /// <exception cref="System.ArgumentNullException">Thrown if the provided layout is null.</exception>
    Task<byte[]> GeneratePdfAsync(PrintLayoutDefinition layout);
}
using DMPS.Shared.Core.Models;
using DMPS.Infrastructure.IO.Printing.Exceptions;

namespace DMPS.Infrastructure.IO.Interfaces;

/// <summary>
/// Defines the contract for a service that sends print jobs to the operating system's print spooler.
/// This abstracts platform-specific printing APIs (e.g., Windows Print API).
/// </summary>
public interface IPrintSpooler
{
    /// <summary>
    /// Submits a print job to the native printing subsystem for processing.
    /// This is a blocking operation until the job is spooled.
    /// </summary>
    /// <param name="job">The data model containing all details of the print job, including the target printer and document content.</param>
    /// <exception cref="PrintSpoolingException">Thrown for any errors during the print spooling process, such as an invalid printer name or a printing subsystem error.</exception>
    /// <exception cref="System.ArgumentNullException">Thrown if the provided job is null.</exception>
    void SpoolJob(PrintJobData job);
}
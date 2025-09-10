using DMPS.Client.Application.DTOs;

namespace DMPS.Client.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles the submission of print and export jobs.
    /// </summary>
    public interface IPrintJobService
    {
        /// <summary>
        /// Submits a print or export job for asynchronous processing by the background service.
        /// </summary>
        /// <param name="jobData">A DTO containing all necessary information for the job.</param>
        /// <returns>A task that represents the asynchronous submission operation.</returns>
        Task SubmitPrintJobAsync(PrintJobData jobData);
    }
}
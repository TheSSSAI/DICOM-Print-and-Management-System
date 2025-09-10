namespace DMPS.Client.Application.DTOs
{
    /// <summary>
    /// Encapsulates all data required by the presentation layer to submit a print or export job.
    /// This DTO is used to pass data from ViewModels to the IPrintJobService.
    /// </summary>
    /// <param name="LayoutDefinition">A serialized representation (e.g., JSON) of the print layout, including templates, margins, orientation, and overlays.</param>
    /// <param name="ImageSopInstanceUids">A list of SOP Instance UIDs for the images to be included in the print job.</param>
    /// <param name="PrinterName">The name of the target Windows printer for a physical print job.</param>
    public sealed record PrintJobData(
        string LayoutDefinition,
        List<string> ImageSopInstanceUids,
        string PrinterName);
}
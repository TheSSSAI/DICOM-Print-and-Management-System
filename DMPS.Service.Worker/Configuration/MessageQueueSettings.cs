using System.ComponentModel.DataAnnotations;

namespace DMPS.Service.Worker.Configuration
{
    /// <summary>
    /// Provides strongly-typed configuration options for the message queue (RabbitMQ) client.
    /// This class is designed to be populated from the "MessageQueue" section of the application's configuration.
    /// </summary>
    public sealed class MessageQueueSettings
    {
        /// <summary>
        /// The configuration section name used in appsettings.json.
        /// </summary>
        public const string SectionName = "MessageQueue";

        /// <summary>
        /// Gets the hostname or IP address of the RabbitMQ server.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The message queue hostname cannot be null or empty.")]
        public string Hostname { get; init; } = "localhost";
        
        /// <summary>
        /// Gets the username for connecting to the RabbitMQ server.
        /// This should be retrieved from a secure source in production.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The message queue username cannot be null or empty.")]
        public string Username { get; init; } = "guest";

        /// <summary>
        /// Gets the password for connecting to the RabbitMQ server.
        /// This should be retrieved from a secure source in production.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The message queue password cannot be null or empty.")]
        public string Password { get; init; } = "guest";

        /// <summary>
        /// Gets the name of the queue used for processing incoming DICOM storage requests.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The DICOM store queue name cannot be null or empty.")]
        public string DicomStoreQueueName { get; init; } = "dicom_store_queue";

        /// <summary>
        /// Gets the name of the queue used for processing print job requests.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The print job queue name cannot be null or empty.")]
        public string PrintJobQueueName { get; init; } = "print_job_queue";

        /// <summary>
        /// Gets the name of the queue used for processing PDF generation requests.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The PDF generation queue name cannot be null or empty.")]
        public string PdfGenerationQueueName { get; init; } = "pdf_generation_queue";
    }
}
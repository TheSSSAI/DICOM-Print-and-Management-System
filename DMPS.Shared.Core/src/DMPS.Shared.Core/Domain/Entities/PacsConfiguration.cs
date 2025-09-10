using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents the configuration for a remote PACS server.
/// This entity corresponds to the 'PacsConfiguration' table in the database.
/// Fulfills requirement REQ-1-038.
/// </summary>
public sealed class PacsConfiguration : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Gets the Application Entity (AE) Title of the remote PACS.
    /// </summary>
    public string AeTitle { get; private set; }

    /// <summary>
    /// Gets the hostname or IP address of the remote PACS.
    /// </summary>
    public string Hostname { get; private set; }

    /// <summary>
    /// Gets the port number for DICOM communication.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the PACS supports C-FIND operations.
    /// </summary>
    public bool SupportsCFind { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the PACS supports C-MOVE operations.
    /// </summary>
    public bool SupportsCMove { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the PACS supports C-STORE operations.
    /// </summary>
    public bool SupportsCStore { get; private set; }

    /// <summary>
    /// Gets the timestamp of when this configuration was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last update to this configuration.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private PacsConfiguration() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacsConfiguration"/> class.
    /// </summary>
    private PacsConfiguration(Guid id, string aeTitle, string hostname, int port, bool supportsCFind, bool supportsCMove, bool supportsCStore) : base(id)
    {
        AeTitle = aeTitle;
        Hostname = hostname;
        Port = port;
        SupportsCFind = supportsCFind;
        SupportsCMove = supportsCMove;
        SupportsCStore = supportsCStore;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    /// <summary>
    /// Creates a new PACS configuration.
    /// </summary>
    /// <param name="aeTitle">The AE Title.</param>
    /// <param name="hostname">The hostname or IP address.</param>
    /// <param name="port">The port number.</param>
    /// <param name="supportsCFind">Indicates support for C-FIND.</param>
    /// <param name="supportsCMove">Indicates support for C-MOVE.</param>
    /// <param name="supportsCStore">Indicates support for C-STORE.</param>
    /// <returns>A new <see cref="PacsConfiguration"/> instance.</returns>
    public static PacsConfiguration Create(string aeTitle, string hostname, int port, bool supportsCFind, bool supportsCMove, bool supportsCStore)
    {
        Guard.Against.NullOrWhiteSpace(aeTitle, nameof(aeTitle));
        Guard.Against.NullOrWhiteSpace(hostname, nameof(hostname));
        Guard.Against.OutOfRange(port, nameof(port), 1, 65535);

        return new PacsConfiguration(Guid.NewGuid(), aeTitle, hostname, port, supportsCFind, supportsCMove, supportsCStore);
    }

    /// <summary>
    /// Updates an existing PACS configuration.
    /// </summary>
    /// <param name="aeTitle">The new AE Title.</param>
    /// <param name="hostname">The new hostname or IP address.</param>
    /// <param name="port">The new port number.</param>
    /// <param name="supportsCFind">The new value for C-FIND support.</param>
    /// <param name="supportsCMove">The new value for C-MOVE support.</param>
    /// <param name="supportsCStore">The new value for C-STORE support.</param>
    public void Update(string aeTitle, string hostname, int port, bool supportsCFind, bool supportsCMove, bool supportsCStore)
    {
        Guard.Against.NullOrWhiteSpace(aeTitle, nameof(aeTitle));
        Guard.Against.NullOrWhiteSpace(hostname, nameof(hostname));
        Guard.Against.OutOfRange(port, nameof(port), 1, 65535);
        
        AeTitle = aeTitle;
        Hostname = hostname;
        Port = port;
        SupportsCFind = supportsCFind;
        SupportsCMove = supportsCMove;
        SupportsCStore = supportsCStore;
        UpdatedAt = DateTime.UtcNow;
    }
}
using DMPS.Shared.Core.Common;
using DMPS.Shared.Core.Domain.Primitives;

namespace DMPS.Shared.Core.Domain.Entities;

/// <summary>
/// Represents a rule for automatically routing incoming C-STORE studies to a specific folder.
/// This entity corresponds to the 'AutoRoutingRule' table in the database.
/// Fulfills requirement REQ-1-037.
/// </summary>
public sealed class AutoRoutingRule : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Gets the descriptive name for the rule.
    /// </summary>
    public string RuleName { get; private set; }

    /// <summary>
    /// Gets the JSON object defining the matching criteria for the rule.
    /// Example: {"SendingAETitle": "MODALITY_AE", "Modality": "CT"}
    /// </summary>
    public string Criteria { get; private set; }

    /// <summary>
    /// Gets the destination folder path where matching studies will be stored.
    /// </summary>
    public string DestinationPath { get; private set; }

    /// <summary>
    /// Gets the order in which rules are evaluated (lower numbers first).
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the rule is currently active.
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Gets the timestamp of when this rule was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last update to this rule.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }
    
    /// <summary>
    /// Private constructor for ORM materialization.
    /// </summary>
    private AutoRoutingRule() : base(Guid.NewGuid()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoRoutingRule"/> class.
    /// </summary>
    private AutoRoutingRule(Guid id, string ruleName, string criteria, string destinationPath, int priority, bool isEnabled) : base(id)
    {
        RuleName = ruleName;
        Criteria = criteria;
        DestinationPath = destinationPath;
        Priority = priority;
        IsEnabled = isEnabled;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    /// <summary>
    /// Creates a new auto-routing rule.
    /// </summary>
    /// <param name="ruleName">The name of the rule.</param>
    /// <param name="criteria">The JSON string representing the matching criteria.</param>
    /// <param name="destinationPath">The destination folder path.</param>
    /// <param name="priority">The evaluation priority.</param>
    /// <param name="isEnabled">Whether the rule is enabled.</param>
    /// <returns>A new <see cref="AutoRoutingRule"/> instance.</returns>
    public static AutoRoutingRule Create(string ruleName, string criteria, string destinationPath, int priority, bool isEnabled)
    {
        Guard.Against.NullOrWhiteSpace(ruleName, nameof(ruleName));
        Guard.Against.NullOrWhiteSpace(criteria, nameof(criteria));
        Guard.Against.NullOrWhiteSpace(destinationPath, nameof(destinationPath));
        // A simple check for JSON structure. More robust validation would be in a domain service.
        Guard.Against.InvalidFormat(criteria, nameof(criteria), c => c.StartsWith("{") && c.EndsWith("}"));
        
        return new AutoRoutingRule(Guid.NewGuid(), ruleName, criteria, destinationPath, priority, isEnabled);
    }

    /// <summary>
    /// Updates an existing auto-routing rule.
    /// </summary>
    /// <param name="ruleName">The updated name of the rule.</param>
    /// <param name="criteria">The updated JSON string representing the matching criteria.</param>
    /// <param name="destinationPath">The updated destination folder path.</param>
    /// <param name="priority">The updated evaluation priority.</param>
    /// <param name="isEnabled">The updated enabled status.</param>
    public void Update(string ruleName, string criteria, string destinationPath, int priority, bool isEnabled)
    {
        Guard.Against.NullOrWhiteSpace(ruleName, nameof(ruleName));
        Guard.Against.NullOrWhiteSpace(criteria, nameof(criteria));
        Guard.Against.NullOrWhiteSpace(destinationPath, nameof(destinationPath));
        Guard.Against.InvalidFormat(criteria, nameof(criteria), c => c.StartsWith("{") && c.EndsWith("}"));

        RuleName = ruleName;
        Criteria = criteria;
        DestinationPath = destinationPath;
        Priority = priority;
        IsEnabled = isEnabled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Enables the routing rule.
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Disables the routing rule.
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
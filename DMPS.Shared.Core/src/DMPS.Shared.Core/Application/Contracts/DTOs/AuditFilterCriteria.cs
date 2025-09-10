// DMPS.Shared.Core/Application/Contracts/DTOs/AuditFilterCriteria.cs
namespace DMPS.Shared.Core.Application.Contracts.DTOs
{
    /// <summary>
    /// A Data Transfer Object that encapsulates the criteria for filtering the audit trail.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to filter by. If null, events for all users are returned.</param>
    /// <param name="StartDate">The start of the date range to filter by.</param>
    /// <param name="EndDate">The end of the date range to filter by.</param>
    public sealed record AuditFilterCriteria(
        Guid? UserId,
        DateTime? StartDate,
        DateTime? EndDate);
}
#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace Common;

/// <summary>
/// Aim: Support ticket entity for user support requests.
/// </summary>
public class SupportTicket
{
	[Key]
	public int Id { get; set; }

	[Required]
	public Guid CustomerId { get; set; }

	[ForeignKey(nameof(CustomerId))]
	public Customer? customer { get; set; }

	[Required]
	public string Message { get; set; } = string.Empty;

	public TicketType Type { get; set; } = TicketType.SupportRequest;

	public TicketSource Source { get; set; } = TicketSource.NotSet;

	public TicketStatus Status { get; set; } = TicketStatus.Created;

	public int? SystemUserId { get; set; }

	[ForeignKey(nameof(SystemUserId))]
	public SystemUser? systemUser { get; set; }

	public string? AdminNotes { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset? UpdatedAt { get; set; }

	public DateTimeOffset? ClosedAt { get; set; }

	public ICollection<SupportTicketEntry> Entries { get; set; } = new List<SupportTicketEntry>();
}

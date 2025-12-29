#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Support ticket entity for user support requests.
/// </summary>
public class SupportTicket
{
	[Key]
	public int Id { get; set; }

	[Required]
	public Guid UserId { get; set; }

	[ForeignKey(nameof(UserId))]
	public Customer? User { get; set; }

	[Required]
	public string Message { get; set; } = string.Empty;

	public SupportEntryType Type { get; set; } = SupportEntryType.SupportRequest;

	public TicketStatus Status { get; set; } = TicketStatus.Open;

	public int? AssignedToId { get; set; }

	[ForeignKey(nameof(AssignedToId))]
	public SystemUser? AssignedTo { get; set; }

	public string? AdminNotes { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime? UpdatedAt { get; set; }

	public DateTime? ClosedAt { get; set; }
}

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

	public SupportEntryType Type { get; set; } = SupportEntryType.SupportRequest;

	public TicketStatus Status { get; set; } = TicketStatus.Open;

	public int? SystemUserId { get; set; }

	[ForeignKey(nameof(SystemUserId))]
	public SystemUser? systemUser { get; set; }

	public string? AdminNotes { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime? UpdatedAt { get; set; }

	public DateTime? ClosedAt { get; set; }

	public ICollection<SupportTicketEntry> Entries { get; set; } = new List<SupportTicketEntry>();
}

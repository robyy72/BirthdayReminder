#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace Common;

/// <summary>
/// Aim: Individual entry/message in a support ticket conversation.
/// </summary>
public class SupportTicketEntry
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int SupportTicketId { get; set; }

	[ForeignKey(nameof(SupportTicketId))]
	public SupportTicket? supportTicket { get; set; }

	[Required]
	public string Message { get; set; } = string.Empty;

	public bool IsFromCustomer { get; set; } = true;

	public int? SystemUserId { get; set; }

	[ForeignKey(nameof(SystemUserId))]
	public SystemUser? systemUser { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

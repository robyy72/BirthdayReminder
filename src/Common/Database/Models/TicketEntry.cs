#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace Common;

/// <summary>
/// Aim: Individual entry/message in a ticket conversation.
/// </summary>
public class TicketEntry
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int TicketId { get; set; }

	[ForeignKey(nameof(TicketId))]
	public Ticket? Ticket { get; set; }

	[Required]
	public string Message { get; set; } = string.Empty;

	public bool IsFromCustomer { get; set; } = true;

	public int? SystemUserId { get; set; }

	[ForeignKey(nameof(SystemUserId))]
	public SystemUser? SystemUser { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

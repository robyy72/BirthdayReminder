#region Usings
using Common;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Filter model for ticket list filtering.
/// </summary>
public class TicketFilter
{
	public TicketStatus? Status { get; set; }
	public TicketType? Type { get; set; }
	public string? SearchText { get; set; }
	public DateTime? FromDate { get; set; }
	public DateTime? ToDate { get; set; }
}

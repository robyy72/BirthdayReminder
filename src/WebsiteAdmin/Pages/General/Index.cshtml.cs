#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for admin dashboard overview.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
	#region Fields
	private readonly HeartbeatService _heartbeatService;
	private readonly TicketService _supportService;
	#endregion

	#region Properties
	public UserStats UserStats { get; set; } = new();
	public int TotalTickets { get; set; }
	public int OpenTickets { get; set; }
	public int AssignedTickets { get; set; }
	public int AdminRepliedTickets { get; set; }
	public int CustomerRepliedTickets { get; set; }
	public int ResolvedTickets { get; set; }
	public int CancelledTickets { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the dashboard page model.
	/// Params: heartbeatService (HeartbeatService), supportService (TicketService).
	/// </summary>
	public IndexModel(HeartbeatService heartbeatService, TicketService supportService)
	{
		_heartbeatService = heartbeatService;
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load dashboard statistics.
	/// Params: None.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync()
	{
		UserStats = await _heartbeatService.GetUserStatsAsync();

		var ticketStats = await _supportService.GetTicketStatsAsync();
		ticketStats.TryGetValue(TicketStatus.Open, out var open);
		ticketStats.TryGetValue(TicketStatus.Assigned, out var assigned);
		ticketStats.TryGetValue(TicketStatus.AdminReplied, out var adminReplied);
		ticketStats.TryGetValue(TicketStatus.CustomerReplied, out var customerReplied);
		ticketStats.TryGetValue(TicketStatus.Resolved, out var resolved);
		ticketStats.TryGetValue(TicketStatus.Cancelled, out var cancelled);

		OpenTickets = created;
		AssignedTickets = assigned;
		AdminRepliedTickets = weAnswered;
		CustomerRepliedTickets = waiting;
		ResolvedTickets = successful;
		CancelledTickets = cancelled;
		TotalTickets = open + assigned + adminReplied + customerReplied + resolved + cancelled;
	}
	#endregion
}

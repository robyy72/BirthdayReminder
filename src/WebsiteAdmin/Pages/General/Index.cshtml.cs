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
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public UserStats UserStats { get; set; } = new();
	public int TotalTickets { get; set; }
	public int CreatedTickets { get; set; }
	public int AssignedTickets { get; set; }
	public int WeAnsweredTickets { get; set; }
	public int WaitingTickets { get; set; }
	public int SuccessfulTickets { get; set; }
	public int CancelledTickets { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the dashboard page model.
	/// Params: heartbeatService (HeartbeatService), supportService (SupportService).
	/// </summary>
	public IndexModel(HeartbeatService heartbeatService, SupportService supportService)
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
		ticketStats.TryGetValue(TicketStatus.Created, out var created);
		ticketStats.TryGetValue(TicketStatus.Assigned, out var assigned);
		ticketStats.TryGetValue(TicketStatus.WeAnswered, out var weAnswered);
		ticketStats.TryGetValue(TicketStatus.WaitingForClientAnswer, out var waiting);
		ticketStats.TryGetValue(TicketStatus.Successful, out var successful);
		ticketStats.TryGetValue(TicketStatus.Cancelled, out var cancelled);

		CreatedTickets = created;
		AssignedTickets = assigned;
		WeAnsweredTickets = weAnswered;
		WaitingTickets = waiting;
		SuccessfulTickets = successful;
		CancelledTickets = cancelled;
		TotalTickets = created + assigned + weAnswered + waiting + successful + cancelled;
	}
	#endregion
}

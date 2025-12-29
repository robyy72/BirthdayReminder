#region Usings
using Common;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for admin dashboard.
/// </summary>
public class IndexModel : PageModel
{
	#region Fields
	private readonly HeartbeatService _heartbeatService;
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public UserStats UserStats { get; set; } = new();
	public Dictionary<TicketStatus, int> TicketStats { get; set; } = [];
	public int OpenTickets { get; set; }
	#endregion

	#region Constructor
	public IndexModel(HeartbeatService heartbeatService, SupportService supportService)
	{
		_heartbeatService = heartbeatService;
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync()
	{
		UserStats = await _heartbeatService.GetUserStatsAsync();
		TicketStats = await _supportService.GetTicketStatsAsync();

		// Calculate open tickets (not closed)
		OpenTickets = TicketStats
			.Where(t => t.Key != TicketStatus.Closed)
			.Sum(t => t.Value);
	}
	#endregion
}

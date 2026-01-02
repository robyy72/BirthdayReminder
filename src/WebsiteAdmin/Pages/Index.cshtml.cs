#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for admin dashboard with ticket management.
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
	public List<SupportTicket> Tickets { get; set; } = [];
	public SupportTicket? SelectedTicket { get; set; }

	[BindProperty(SupportsGet = true)]
	public TicketFilter Filter { get; set; } = new();

	[BindProperty(SupportsGet = true)]
	public int? SelectedId { get; set; }
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
		Tickets = await _supportService.GetFilteredTicketsAsync(Filter);

		if (SelectedId.HasValue)
		{
			SelectedTicket = await _supportService.GetTicketByIdAsync(SelectedId.Value);
		}
		else if (Tickets.Count > 0)
		{
			SelectedTicket = Tickets[0];
			SelectedId = SelectedTicket.Id;
		}
	}

	public async Task<IActionResult> OnPostUpdateStatusAsync(int ticketId, TicketStatus status)
	{
		await _supportService.UpdateTicketStatusAsync(ticketId, status);

		return RedirectToPage(new { SelectedId = ticketId });
	}

	public async Task<IActionResult> OnPostAddEntryAsync(int ticketId, string message)
	{
		await _supportService.AddTicketEntryAsync(ticketId, message, false, User.Identity?.Name);

		return RedirectToPage(new { SelectedId = ticketId });
	}
	#endregion
}

#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for support tickets list.
/// </summary>
[Authorize]
public class TicketsModel : PageModel
{
	#region Fields
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public List<SupportTicket> Tickets { get; set; } = [];
	public string? StatusFilter { get; set; }
	#endregion

	#region Constructor
	public TicketsModel(SupportService supportService)
	{
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync(string? status)
	{
		StatusFilter = status;

		TicketStatus? ticketStatus = status switch
		{
			"Open" => TicketStatus.Open,
			"InProgress" => TicketStatus.InProgress,
			"Closed" => TicketStatus.Closed,
			_ => null
		};

		Tickets = await _supportService.GetTicketsAsync(ticketStatus);
	}
	#endregion
}

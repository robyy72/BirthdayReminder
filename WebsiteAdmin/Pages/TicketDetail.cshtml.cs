#region Usings
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for support ticket detail.
/// </summary>
public class TicketDetailModel : PageModel
{
	#region Fields
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public SupportTicket? Ticket { get; set; }
	#endregion

	#region Constructor
	public TicketDetailModel(SupportService supportService)
	{
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync(int id)
	{
		Ticket = await _supportService.GetTicketByIdAsync(id);
	}

	public async Task<IActionResult> OnPostAsync(int ticketId, string status, string? adminNotes)
	{
		var ticketStatus = status switch
		{
			"Open" => TicketStatus.Open,
			"InProgress" => TicketStatus.InProgress,
			"Closed" => TicketStatus.Closed,
			_ => TicketStatus.Open
		};

		await _supportService.UpdateTicketStatusAsync(ticketId, ticketStatus, adminNotes);

		return RedirectToPage("/TicketDetail", new { id = ticketId });
	}
	#endregion
}

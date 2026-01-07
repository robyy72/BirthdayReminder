#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.Tickets;

/// <summary>
/// Aim: Page model for support tickets with filtering and detail view.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
	#region Fields
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public List<SupportTicket> Tickets { get; set; } = [];
	public SupportTicket? SelectedTicket { get; set; }

	[BindProperty(SupportsGet = true)]
	public TicketFilter Filter { get; set; } = new();

	[BindProperty(SupportsGet = true)]
	public int? SelectedId { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the tickets page model.
	/// Params: supportService (SupportService) - Service for ticket operations.
	/// </summary>
	public IndexModel(SupportService supportService)
	{
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load tickets with filters and selected ticket details.
	/// Params: None.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync()
	{
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

	/// <summary>
	/// Aim: Update the status of a ticket.
	/// Params: ticketId (int) - Ticket ID, status (TicketStatus) - New status.
	/// Return (IActionResult): Redirect to page with selected ticket.
	/// </summary>
	public async Task<IActionResult> OnPostUpdateStatusAsync(int ticketId, TicketStatus status)
	{
		await _supportService.UpdateTicketStatusAsync(ticketId, status);

		return RedirectToPage(new { SelectedId = ticketId });
	}

	/// <summary>
	/// Aim: Add a new entry to a ticket conversation.
	/// Params: ticketId (int) - Ticket ID, message (string) - Message content.
	/// Return (IActionResult): Redirect to page with selected ticket.
	/// </summary>
	public async Task<IActionResult> OnPostAddEntryAsync(int ticketId, string message)
	{
		await _supportService.AddTicketEntryAsync(ticketId, message, false, User.Identity?.Name);

		return RedirectToPage(new { SelectedId = ticketId });
	}
	#endregion
}

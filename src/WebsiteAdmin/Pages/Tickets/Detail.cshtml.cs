#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.Tickets;

/// <summary>
/// Aim: Page model for support ticket detail.
/// </summary>
[Authorize]
public class DetailModel : PageModel
{
	#region Fields
	private readonly SupportService _supportService;
	#endregion

	#region Properties
	public SupportTicket? Ticket { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the ticket detail page model.
	/// Params: supportService (SupportService) - Service for ticket operations.
	/// </summary>
	public DetailModel(SupportService supportService)
	{
		_supportService = supportService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load ticket by ID.
	/// Params: id (int) - Ticket ID.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync(int id)
	{
		Ticket = await _supportService.GetTicketByIdAsync(id);
	}

	/// <summary>
	/// Aim: Update ticket status and notes.
	/// Params: ticketId (int), status (string), adminNotes (string).
	/// Return (IActionResult): Redirect to detail page.
	/// </summary>
	public async Task<IActionResult> OnPostAsync(int ticketId, string status, string? adminNotes)
	{
		if (!Enum.TryParse<TicketStatus>(status, out var ticketStatus))
		{
			ticketStatus = TicketStatus.Created;
		}

		await _supportService.UpdateTicketStatusAsync(ticketId, ticketStatus, adminNotes);

		return RedirectToPage("/Tickets/Detail", new { id = ticketId });
	}
	#endregion
}

#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.Customers;

/// <summary>
/// Aim: Page model for customers list.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
	#region Fields
	private readonly HeartbeatService _heartbeatService;
	#endregion

	#region Properties
	public List<Customer> Users { get; set; } = [];
	public UserStats Stats { get; set; } = new();
	#endregion

	#region Constructor
	public IndexModel(HeartbeatService heartbeatService)
	{
		_heartbeatService = heartbeatService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync()
	{
		Users = await _heartbeatService.GetAllUsersAsync();
		Stats = await _heartbeatService.GetUserStatsAsync();
	}
	#endregion
}

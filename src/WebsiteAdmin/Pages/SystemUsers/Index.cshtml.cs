#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.SystemUsers;

/// <summary>
/// Aim: Page model for system users list.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Properties
	public List<SystemUser> Users { get; set; } = [];
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the system users page model.
	/// Params: authService (AuthService) - Service for auth operations.
	/// </summary>
	public IndexModel(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load all system users.
	/// Params: None.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync()
	{
		Users = await _authService.GetAllUsersAsync();
	}
	#endregion
}

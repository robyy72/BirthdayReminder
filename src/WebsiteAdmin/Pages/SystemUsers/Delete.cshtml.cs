#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.SystemUsers;

/// <summary>
/// Aim: Page model for deleting system user.
/// </summary>
[Authorize]
public class DeleteModel : PageModel
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Properties
	public SystemUser? SystemUser { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the delete page model.
	/// Params: authService (AuthService) - Service for auth operations.
	/// </summary>
	public DeleteModel(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load user for confirmation.
	/// Params: id (int) - User ID.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync(int id)
	{
		SystemUser = await _authService.GetUserByIdAsync(id);
	}

	/// <summary>
	/// Aim: Delete user.
	/// Params: id (int) - User ID.
	/// Return (IActionResult): Redirect to index.
	/// </summary>
	public async Task<IActionResult> OnPostAsync(int id)
	{
		await _authService.DeleteUserAsync(id);

		return RedirectToPage("/SystemUsers/Index");
	}
	#endregion
}

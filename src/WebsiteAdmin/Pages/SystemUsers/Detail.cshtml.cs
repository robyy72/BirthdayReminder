#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.SystemUsers;

/// <summary>
/// Aim: Page model for system user detail/edit.
/// </summary>
[Authorize]
public class DetailModel : PageModel
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Properties
	[BindProperty]
	public SystemUser? SystemUser { get; set; }
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the detail page model.
	/// Params: authService (AuthService) - Service for auth operations.
	/// </summary>
	public DetailModel(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load user by ID.
	/// Params: id (int) - User ID.
	/// Return: Task.
	/// </summary>
	public async Task OnGetAsync(int id)
	{
		SystemUser = await _authService.GetUserByIdAsync(id);
	}

	/// <summary>
	/// Aim: Update user details.
	/// Return (IActionResult): Redirect to detail page.
	/// </summary>
	public async Task<IActionResult> OnPostAsync()
	{
		if (SystemUser == null)
		{
			return NotFound();
		}

		await _authService.UpdateUserAsync(SystemUser.Id, SystemUser.Email, SystemUser.DisplayName, SystemUser.IsActive);

		return RedirectToPage("/SystemUsers/Detail", new { id = SystemUser.Id });
	}

	/// <summary>
	/// Aim: Change user password.
	/// Params: userId (int), newPassword (string).
	/// Return (IActionResult): Redirect to detail page.
	/// </summary>
	public async Task<IActionResult> OnPostChangePasswordAsync(int userId, string newPassword)
	{
		await _authService.ChangePasswordAsync(userId, newPassword);

		return RedirectToPage("/SystemUsers/Detail", new { id = userId });
	}
	#endregion
}

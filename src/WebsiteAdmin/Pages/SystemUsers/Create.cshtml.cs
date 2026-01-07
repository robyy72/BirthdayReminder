#region Usings
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.SystemUsers;

/// <summary>
/// Aim: Page model for creating system user.
/// </summary>
[Authorize]
public class CreateModel : PageModel
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Properties
	[BindProperty]
	public string Email { get; set; } = string.Empty;

	[BindProperty]
	public string? DisplayName { get; set; }

	[BindProperty]
	public string Password { get; set; } = string.Empty;
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Initialize the create page model.
	/// Params: authService (AuthService) - Service for auth operations.
	/// </summary>
	public CreateModel(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Display create form.
	/// </summary>
	public void OnGet()
	{
	}

	/// <summary>
	/// Aim: Create new user.
	/// Return (IActionResult): Redirect to index.
	/// </summary>
	public async Task<IActionResult> OnPostAsync()
	{
		await _authService.CreateUserAsync(Email, Password, DisplayName);

		return RedirectToPage("/SystemUsers/Index");
	}
	#endregion
}

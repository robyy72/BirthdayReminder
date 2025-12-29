#region Usings
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for admin login.
/// </summary>
public class LoginModel : PageModel
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Properties
	public string? ErrorMessage { get; set; }
	#endregion

	#region Constructor
	public LoginModel(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Handlers
	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostAsync(string username, string password)
	{
		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
		{
			ErrorMessage = "Please enter username and password";
			return Page();
		}

		var token = await _authService.LoginAsync(username, password);
		if (token == null)
		{
			ErrorMessage = "Invalid credentials";
			return Page();
		}

		// Store token in cookie for subsequent requests
		Response.Cookies.Append("AuthToken", token, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTimeOffset.UtcNow.AddHours(8)
		});

		return RedirectToPage("/Index");
	}
	#endregion
}

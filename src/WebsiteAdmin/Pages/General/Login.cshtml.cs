#region Usings
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
	public IActionResult OnGet()
	{
		// If already logged in, redirect to dashboard
		if (User.Identity?.IsAuthenticated == true)
		{
			return RedirectToPage("/Index");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(string email, string password)
	{
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			ErrorMessage = "Please enter email and password";
			return Page();
		}

		var user = await _authService.ValidateCredentialsAsync(email, password);
		if (user == null)
		{
			ErrorMessage = "Invalid credentials";
			return Page();
		}

		// Create claims
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email)
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
		};

		await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(claimsIdentity),
			authProperties);

		return RedirectToPage("/Index");
	}
	#endregion
}

#region Usings
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for admin logout.
/// </summary>
public class LogoutModel : PageModel
{
	public async Task<IActionResult> OnGetAsync()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToPage("/Login");
	}
}

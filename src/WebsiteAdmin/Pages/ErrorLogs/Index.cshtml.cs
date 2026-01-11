#region Usings
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages.ErrorLogs;

/// <summary>
/// Aim: Page model for error logs display with Sentry integration info.
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
	#region Properties
	/// <summary>
	/// Aim: URL to the Sentry dashboard for this project.
	/// </summary>
	public string SentryDashboardUrl { get; set; } = "https://birthday-reminder-app.sentry.io/issues/";
	#endregion

	#region Handlers
	/// <summary>
	/// Aim: Load the error logs page.
	/// </summary>
	public void OnGet()
	{
	}
	#endregion
}

#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for managing email text templates.
/// </summary>
[Authorize]
public class EmailTextsModel : PageModel
{
	#region Fields
	private readonly TextsService _textsService;
	#endregion

	#region Properties
	public List<EmailText> EmailTexts { get; set; } = [];

	[BindProperty]
	public EmailText? SelectedText { get; set; }

	[BindProperty(SupportsGet = true)]
	public int? SelectedId { get; set; }
	#endregion

	#region Constructor
	public EmailTextsModel(TextsService textsService)
	{
		_textsService = textsService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync()
	{
		EmailTexts = await _textsService.GetEmailTextsAsync();

		if (SelectedId.HasValue)
		{
			SelectedText = await _textsService.GetEmailTextByIdAsync(SelectedId.Value);
		}
		else if (EmailTexts.Count > 0)
		{
			SelectedText = EmailTexts[0];
			SelectedId = SelectedText.Id;
		}
	}

	public async Task<IActionResult> OnPostCreateAsync(string name, string subject, string content, bool isHtml = true)
	{
		var text = new EmailText
		{
			Name = name,
			Subject = subject,
			Content = content,
			IsHtml = isHtml,
			IsActive = true
		};

		var id = await _textsService.CreateEmailTextAsync(text);

		return RedirectToPage(new { SelectedId = id });
	}

	public async Task<IActionResult> OnPostUpdateAsync()
	{
		if (SelectedText == null)
		{
			return RedirectToPage();
		}

		await _textsService.UpdateEmailTextAsync(SelectedText);

		return RedirectToPage(new { SelectedId = SelectedText.Id });
	}

	public async Task<IActionResult> OnPostDeleteAsync(int id)
	{
		await _textsService.DeleteEmailTextAsync(id);

		return RedirectToPage();
	}
	#endregion
}

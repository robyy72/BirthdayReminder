#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
#endregion

namespace WebsiteAdmin.Pages;

/// <summary>
/// Aim: Page model for managing messenger text templates.
/// </summary>
[Authorize]
public class MessengerTextsModel : PageModel
{
	#region Fields
	private readonly TextsService _textsService;
	#endregion

	#region Properties
	public List<MessengerText> MessengerTexts { get; set; } = [];

	[BindProperty]
	public MessengerText? SelectedText { get; set; }

	[BindProperty(SupportsGet = true)]
	public int? SelectedId { get; set; }
	#endregion

	#region Constructor
	public MessengerTextsModel(TextsService textsService)
	{
		_textsService = textsService;
	}
	#endregion

	#region Handlers
	public async Task OnGetAsync()
	{
		MessengerTexts = await _textsService.GetMessengerTextsAsync();

		if (SelectedId.HasValue)
		{
			SelectedText = await _textsService.GetMessengerTextByIdAsync(SelectedId.Value);
		}
		else if (MessengerTexts.Count > 0)
		{
			SelectedText = MessengerTexts[0];
			SelectedId = SelectedText.Id;
		}
	}

	public async Task<IActionResult> OnPostCreateAsync(string name, PreferredChannel channel, string? previewText, string content)
	{
		var text = new MessengerText
		{
			Name = name,
			Channel = channel,
			PreviewText = previewText,
			Content = content,
			IsActive = true
		};

		var id = await _textsService.CreateMessengerTextAsync(text);

		return RedirectToPage(new { SelectedId = id });
	}

	public async Task<IActionResult> OnPostUpdateAsync()
	{
		if (SelectedText == null)
		{
			return RedirectToPage();
		}

		await _textsService.UpdateMessengerTextAsync(SelectedText);

		return RedirectToPage(new { SelectedId = SelectedText.Id });
	}

	public async Task<IActionResult> OnPostDeleteAsync(int id)
	{
		await _textsService.DeleteMessengerTextAsync(id);

		return RedirectToPage();
	}
	#endregion
}

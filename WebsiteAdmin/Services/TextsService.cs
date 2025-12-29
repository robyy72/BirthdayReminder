#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for managing email and messenger text templates.
/// </summary>
public class TextsService
{
	#region Fields
	private readonly CoreDbContext _db;
	#endregion

	#region Constructor
	public TextsService(CoreDbContext db)
	{
		_db = db;
	}
	#endregion

	#region Email Text Methods
	/// <summary>
	/// Aim: Get all email texts.
	/// Return: List of email texts.
	/// </summary>
	public async Task<List<EmailText>> GetEmailTextsAsync()
	{
		var texts = await _db.EmailTexts
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return texts;
	}

	/// <summary>
	/// Aim: Get email text by ID.
	/// Params: id - email text ID.
	/// Return: Email text or null.
	/// </summary>
	public async Task<EmailText?> GetEmailTextByIdAsync(int id)
	{
		var text = await _db.EmailTexts.FindAsync(id);

		return text;
	}

	/// <summary>
	/// Aim: Create new email text.
	/// Params: text - email text to create.
	/// Return: Created email text ID.
	/// </summary>
	public async Task<int> CreateEmailTextAsync(EmailText text)
	{
		text.CreatedAt = DateTimeOffset.UtcNow;
		_db.EmailTexts.Add(text);
		await _db.SaveChangesAsync();

		return text.Id;
	}

	/// <summary>
	/// Aim: Update email text.
	/// Params: text - email text with updates.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> UpdateEmailTextAsync(EmailText text)
	{
		var existing = await _db.EmailTexts.FindAsync(text.Id);
		if (existing == null)
		{
			return false;
		}

		existing.Name = text.Name;
		existing.Subject = text.Subject;
		existing.Content = text.Content;
		existing.IsHtml = text.IsHtml;
		existing.IsActive = text.IsActive;
		existing.UpdatedAt = DateTimeOffset.UtcNow;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Delete email text.
	/// Params: id - email text ID.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> DeleteEmailTextAsync(int id)
	{
		var text = await _db.EmailTexts.FindAsync(id);
		if (text == null)
		{
			return false;
		}

		_db.EmailTexts.Remove(text);
		await _db.SaveChangesAsync();
		return true;
	}
	#endregion

	#region Messenger Text Methods
	/// <summary>
	/// Aim: Get all messenger texts.
	/// Return: List of messenger texts.
	/// </summary>
	public async Task<List<MessengerText>> GetMessengerTextsAsync()
	{
		var texts = await _db.MessengerTexts
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return texts;
	}

	/// <summary>
	/// Aim: Get messenger text by ID.
	/// Params: id - messenger text ID.
	/// Return: Messenger text or null.
	/// </summary>
	public async Task<MessengerText?> GetMessengerTextByIdAsync(int id)
	{
		var text = await _db.MessengerTexts.FindAsync(id);

		return text;
	}

	/// <summary>
	/// Aim: Create new messenger text.
	/// Params: text - messenger text to create.
	/// Return: Created messenger text ID.
	/// </summary>
	public async Task<int> CreateMessengerTextAsync(MessengerText text)
	{
		text.CreatedAt = DateTimeOffset.UtcNow;
		_db.MessengerTexts.Add(text);
		await _db.SaveChangesAsync();

		return text.Id;
	}

	/// <summary>
	/// Aim: Update messenger text.
	/// Params: text - messenger text with updates.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> UpdateMessengerTextAsync(MessengerText text)
	{
		var existing = await _db.MessengerTexts.FindAsync(text.Id);
		if (existing == null)
		{
			return false;
		}

		existing.Name = text.Name;
		existing.Content = text.Content;
		existing.Channel = text.Channel;
		existing.PreviewText = text.PreviewText;
		existing.IsActive = text.IsActive;
		existing.UpdatedAt = DateTimeOffset.UtcNow;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Delete messenger text.
	/// Params: id - messenger text ID.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> DeleteMessengerTextAsync(int id)
	{
		var text = await _db.MessengerTexts.FindAsync(id);
		if (text == null)
		{
			return false;
		}

		_db.MessengerTexts.Remove(text);
		await _db.SaveChangesAsync();
		return true;
	}
	#endregion
}

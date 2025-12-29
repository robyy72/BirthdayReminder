#region Usings
using System.Reflection;
using Microsoft.EntityFrameworkCore;
#endregion

namespace Common;

/// <summary>
/// Aim: Initialize database with seed data at runtime.
/// </summary>
public static class DbContextInit
{
	#region Constants
	private const string MESSAGE_TEXTS_PATH = "Common.Resources.Message-Texts";
	private const string DEFAULT_LANGUAGE = "en";
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Seed the database with initial data if not exists.
	/// Params: db - the database context, language - language code (en, de).
	/// Return: None.
	/// </summary>
	public static async Task SeedAsync(CoreDbContext db, string language = DEFAULT_LANGUAGE)
	{
		await SeedSystemUsersAsync(db);
		await SeedEmailTextsAsync(db, language);
		await SeedMessengerTextsAsync(db, language);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Aim: Seed default admin user.
	/// Params: db - the database context.
	/// Return: None.
	/// </summary>
	private static async Task SeedSystemUsersAsync(CoreDbContext db)
	{
		if (await db.SystemUsers.AnyAsync()) return;

		var admin = new SystemUser
		{
			Email = "admin@birthday-reminder.online",
			PasswordHash = CryptHelper.HashPassword("admin123"),
			DisplayName = "Admin",
			CreatedAt = DateTimeOffset.UtcNow,
			IsActive = true
		};

		db.SystemUsers.Add(admin);
		await db.SaveChangesAsync();
	}

	/// <summary>
	/// Aim: Seed email templates from resource files.
	/// Params: db - the database context, language - language code.
	/// Return: None.
	/// </summary>
	private static async Task SeedEmailTextsAsync(CoreDbContext db, string language)
	{
		if (await db.EmailTexts.AnyAsync()) return;

		var templates = new List<EmailText>
		{
			CreateEmailText("Welcome", language),
			CreateEmailText("BirthdayReminder", language),
			CreateEmailText("SupportTicketCreated", language),
			CreateEmailText("SupportTicketResponse", language),
			CreateEmailText("SupportTicketClosed", language)
		};

		db.EmailTexts.AddRange(templates);
		await db.SaveChangesAsync();
	}

	/// <summary>
	/// Aim: Seed messenger templates from resource files.
	/// Params: db - the database context, language - language code.
	/// Return: None.
	/// </summary>
	private static async Task SeedMessengerTextsAsync(CoreDbContext db, string language)
	{
		if (await db.MessengerTexts.AnyAsync()) return;

		var templates = new List<MessengerText>
		{
			CreateMessengerText("BirthdayReminder", PreferredChannel.WhatsApp, "Birthday reminder for {Name}", language),
			CreateMessengerText("BirthdayReminder", PreferredChannel.Sms, "Birthday: {Name}", language),
			CreateMessengerText("BirthdayReminder", PreferredChannel.Signal, "Birthday reminder for {Name}", language),
			CreateMessengerText("SupportResponse", PreferredChannel.WhatsApp, "Support Ticket #{TicketId}", language),
			CreateMessengerText("SupportResponse", PreferredChannel.Sms, "Ticket #{TicketId}", language)
		};

		db.MessengerTexts.AddRange(templates);
		await db.SaveChangesAsync();
	}

	/// <summary>
	/// Aim: Create an EmailText from resource files.
	/// Params: templateName - name of the template, language - language code.
	/// Return (EmailText): The created email text.
	/// </summary>
	private static EmailText CreateEmailText(string templateName, string language)
	{
		var subject = ReadResourceFile($"{language}.Email-{templateName}-Subject.txt");
		var body = ReadResourceFile($"{language}.Email-{templateName}-Body.html");

		var emailText = new EmailText
		{
			Name = templateName,
			Subject = subject,
			Content = body,
			IsHtml = true,
			IsActive = true,
			CreatedAt = DateTimeOffset.UtcNow
		};

		return emailText;
	}

	/// <summary>
	/// Aim: Create a MessengerText from resource files.
	/// Params: templateName - name of template, channel - messenger channel, previewText - preview text, language - language code.
	/// Return (MessengerText): The created messenger text.
	/// </summary>
	private static MessengerText CreateMessengerText(string templateName, PreferredChannel channel, string previewText, string language)
	{
		var channelName = channel.ToString();
		var content = ReadResourceFile($"{language}.Messenger-{templateName}-{channelName}.txt");

		var messengerText = new MessengerText
		{
			Name = $"{templateName} {channelName}",
			Channel = channel,
			PreviewText = previewText,
			Content = content,
			IsActive = true,
			CreatedAt = DateTimeOffset.UtcNow
		};

		return messengerText;
	}

	/// <summary>
	/// Aim: Read content from embedded resource file.
	/// Params: resourceName - relative resource name (e.g., "en.Email-Welcome-Body.html").
	/// Return (string): The file content.
	/// </summary>
	private static string ReadResourceFile(string resourceName)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var fullResourceName = $"{MESSAGE_TEXTS_PATH}.{resourceName}";

		using var stream = assembly.GetManifestResourceStream(fullResourceName);
		if (stream == null)
		{
			throw new InvalidOperationException($"Resource not found: {fullResourceName}");
		}

		using var reader = new StreamReader(stream);
		var content = reader.ReadToEnd();
		return content;
	}
	#endregion
}

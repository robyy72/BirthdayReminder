#region Usings
using Microsoft.EntityFrameworkCore;
#endregion

namespace Common;

/// <summary>
/// Aim: Initialize database with seed data at runtime.
/// </summary>
public static class DbContextInit
{
	#region Public Methods
	/// <summary>
	/// Aim: Seed the database with initial data if not exists.
	/// Params: db - the database context.
	/// Return: None.
	/// </summary>
	public static async Task SeedAsync(CoreDbContext db)
	{
		await SeedSystemUsersAsync(db);
		await SeedEmailTextsAsync(db);
		await SeedMessengerTextsAsync(db);
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
	/// Aim: Seed email templates.
	/// Params: db - the database context.
	/// Return: None.
	/// </summary>
	private static async Task SeedEmailTextsAsync(CoreDbContext db)
	{
		if (await db.EmailTexts.AnyAsync()) return;

		var templates = new List<EmailText>
		{
			new EmailText
			{
				Name = "Welcome",
				Subject = "Welcome to Birthday Reminder!",
				Content = @"<h1>Welcome to Birthday Reminder!</h1>
<p>Thank you for joining Birthday Reminder. We're excited to help you never forget an important birthday again.</p>
<p>Get started by adding your first birthday reminder in the app.</p>
<p>Best regards,<br>The Birthday Reminder Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new EmailText
			{
				Name = "Birthday Reminder",
				Subject = "Birthday Reminder: {Name}'s birthday is coming up!",
				Content = @"<h1>Birthday Reminder</h1>
<p>Don't forget! <strong>{Name}</strong>'s birthday is on <strong>{Date}</strong>.</p>
<p>You have {DaysLeft} days to prepare a gift or send your wishes.</p>
<p>Best regards,<br>The Birthday Reminder Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new EmailText
			{
				Name = "Support Ticket Created",
				Subject = "Support Ticket #{TicketId} - We received your request",
				Content = @"<h1>Support Request Received</h1>
<p>Thank you for contacting us. We have received your support request.</p>
<p><strong>Ticket ID:</strong> #{TicketId}<br>
<strong>Subject:</strong> {Subject}</p>
<p>Our team will review your request and get back to you as soon as possible.</p>
<p>Best regards,<br>The Birthday Reminder Support Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new EmailText
			{
				Name = "Support Ticket Response",
				Subject = "Support Ticket #{TicketId} - New response",
				Content = @"<h1>New Response to Your Support Ticket</h1>
<p>There is a new response to your support ticket #{TicketId}.</p>
<p><strong>Response:</strong></p>
<blockquote>{Message}</blockquote>
<p>If you have any further questions, please reply to this email.</p>
<p>Best regards,<br>The Birthday Reminder Support Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new EmailText
			{
				Name = "Support Ticket Closed",
				Subject = "Support Ticket #{TicketId} - Closed",
				Content = @"<h1>Support Ticket Closed</h1>
<p>Your support ticket #{TicketId} has been closed.</p>
<p>If you need further assistance, feel free to open a new support request in the app.</p>
<p>Thank you for using Birthday Reminder!</p>
<p>Best regards,<br>The Birthday Reminder Support Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			}
		};

		db.EmailTexts.AddRange(templates);
		await db.SaveChangesAsync();
	}

	/// <summary>
	/// Aim: Seed messenger templates.
	/// Params: db - the database context.
	/// Return: None.
	/// </summary>
	private static async Task SeedMessengerTextsAsync(CoreDbContext db)
	{
		if (await db.MessengerTexts.AnyAsync()) return;

		var templates = new List<MessengerText>
		{
			new MessengerText
			{
				Name = "Birthday Reminder WhatsApp",
				Channel = PreferredChannel.WhatsApp,
				PreviewText = "Birthday reminder for {Name}",
				Content = "Hey! Just a friendly reminder: {Name}'s birthday is on {Date}. You have {DaysLeft} days left to prepare!",
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new MessengerText
			{
				Name = "Birthday Reminder SMS",
				Channel = PreferredChannel.Sms,
				PreviewText = "Birthday: {Name}",
				Content = "Birthday Reminder: {Name}'s birthday is on {Date}. {DaysLeft} days left!",
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new MessengerText
			{
				Name = "Birthday Reminder Signal",
				Channel = PreferredChannel.Signal,
				PreviewText = "Birthday reminder for {Name}",
				Content = "Hey! Just a friendly reminder: {Name}'s birthday is on {Date}. You have {DaysLeft} days left to prepare!",
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new MessengerText
			{
				Name = "Support Response WhatsApp",
				Channel = PreferredChannel.WhatsApp,
				PreviewText = "Support Ticket #{TicketId}",
				Content = "New response to your support ticket #{TicketId}:\n\n{Message}\n\nReply in the app for further assistance.",
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			},
			new MessengerText
			{
				Name = "Support Response SMS",
				Channel = PreferredChannel.Sms,
				PreviewText = "Ticket #{TicketId}",
				Content = "Ticket #{TicketId} update: {Message}",
				IsActive = true,
				CreatedAt = DateTimeOffset.UtcNow
			}
		};

		db.MessengerTexts.AddRange(templates);
		await db.SaveChangesAsync();
	}
	#endregion
}

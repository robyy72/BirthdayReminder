#region Usings
using Microsoft.EntityFrameworkCore;
#endregion

namespace Common;

/// <summary>
/// Aim: Unified database context for all projects.
/// </summary>
public class CoreDbContext : DbContext
{
	public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
	{
	}

	public DbSet<Customer> Customers { get; set; } = null!;
	public DbSet<SystemUser> SystemUsers { get; set; } = null!;
	public DbSet<SupportTicket> SupportTickets { get; set; } = null!;
	public DbSet<SupportTicketEntry> SupportTicketEntries { get; set; } = null!;
	public DbSet<EmailText> EmailTexts { get; set; } = null!;
	public DbSet<MessengerText> MessengerTexts { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Customer configuration
		modelBuilder.Entity<Customer>(entity =>
		{
			entity.HasIndex(e => e.Email);
			entity.HasIndex(e => e.PhoneNumber);
		});

		// SystemUser configuration
		modelBuilder.Entity<SystemUser>(entity =>
		{
			entity.HasIndex(e => e.Email).IsUnique();
		});

		// SupportTicket configuration
		modelBuilder.Entity<SupportTicket>(entity =>
		{
			entity.HasOne(t => t.customer)
				.WithMany()
				.HasForeignKey(t => t.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(t => t.systemUser)
				.WithMany()
				.HasForeignKey(t => t.SystemUserId)
				.OnDelete(DeleteBehavior.SetNull);

			entity.HasIndex(e => e.Status);
			entity.HasIndex(e => e.CreatedAt);
		});

		// SupportTicketEntry configuration
		modelBuilder.Entity<SupportTicketEntry>(entity =>
		{
			entity.HasOne(e => e.supportTicket)
				.WithMany(t => t.Entries)
				.HasForeignKey(e => e.SupportTicketId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.systemUser)
				.WithMany()
				.HasForeignKey(e => e.SystemUserId)
				.OnDelete(DeleteBehavior.SetNull);
		});

		// Seed default admin user (password: admin123 - change in production!)
		modelBuilder.Entity<SystemUser>().HasData(new SystemUser
		{
			Id = 1,
			Email = "admin@birthday-reminder.online",
			PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
			DisplayName = "Admin",
			CreatedAt = DateTime.UtcNow,
			IsActive = true
		});

		// Seed email templates
		modelBuilder.Entity<EmailText>().HasData(
			new EmailText
			{
				Id = 1,
				Name = "Welcome",
				Subject = "Welcome to Birthday Reminder!",
				Content = @"<h1>Welcome to Birthday Reminder!</h1>
<p>Thank you for joining Birthday Reminder. We're excited to help you never forget an important birthday again.</p>
<p>Get started by adding your first birthday reminder in the app.</p>
<p>Best regards,<br>The Birthday Reminder Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new EmailText
			{
				Id = 2,
				Name = "Birthday Reminder",
				Subject = "Birthday Reminder: {Name}'s birthday is coming up!",
				Content = @"<h1>Birthday Reminder</h1>
<p>Don't forget! <strong>{Name}</strong>'s birthday is on <strong>{Date}</strong>.</p>
<p>You have {DaysLeft} days to prepare a gift or send your wishes.</p>
<p>Best regards,<br>The Birthday Reminder Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new EmailText
			{
				Id = 3,
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
				CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new EmailText
			{
				Id = 4,
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
				CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			},
			new EmailText
			{
				Id = 5,
				Name = "Support Ticket Closed",
				Subject = "Support Ticket #{TicketId} - Closed",
				Content = @"<h1>Support Ticket Closed</h1>
<p>Your support ticket #{TicketId} has been closed.</p>
<p>If you need further assistance, feel free to open a new support request in the app.</p>
<p>Thank you for using Birthday Reminder!</p>
<p>Best regards,<br>The Birthday Reminder Support Team</p>",
				IsHtml = true,
				IsActive = true,
				CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			}
		);
	}
}

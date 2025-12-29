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
	}
}

#region Usings
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Database context for admin backend with SQLite.
/// </summary>
public class AdminDbContext : DbContext
{
	public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
	{
	}

	public DbSet<SystemUser> SystemUsers { get; set; } = null!;
	public DbSet<AppUser> AppUsers { get; set; } = null!;
	public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// SystemUser configuration
		modelBuilder.Entity<SystemUser>(entity =>
		{
			entity.HasIndex(e => e.Email).IsUnique();
		});

		// AppUser configuration
		modelBuilder.Entity<AppUser>(entity =>
		{
			entity.HasIndex(e => e.Email);
			entity.HasIndex(e => e.PhoneNumber);
		});

		// SupportTicket configuration
		modelBuilder.Entity<SupportTicket>(entity =>
		{
			entity.HasOne(t => t.User)
				.WithMany(u => u.SupportTickets)
				.HasForeignKey(t => t.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(t => t.AssignedTo)
				.WithMany()
				.HasForeignKey(t => t.AssignedToId)
				.OnDelete(DeleteBehavior.SetNull);

			entity.HasIndex(e => e.Status);
			entity.HasIndex(e => e.CreatedAt);
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

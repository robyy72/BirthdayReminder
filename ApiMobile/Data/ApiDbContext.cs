#region Usings
using Microsoft.EntityFrameworkCore;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Database context for mobile API with SQLite.
/// Note: Shares database with WebsiteAdmin for AppUsers and SupportTickets.
/// </summary>
public class ApiDbContext : DbContext
{
	public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
	{
	}

	public DbSet<AppUser> AppUsers { get; set; } = null!;
	public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

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

			entity.HasIndex(e => e.Status);
			entity.HasIndex(e => e.CreatedAt);
		});
	}
}

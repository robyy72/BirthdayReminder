#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Database context for mobile API with SQLite.
/// Note: Shares database with WebsiteAdmin for Customers and SupportTickets.
/// </summary>
public class ApiDbContext : DbContext
{
	public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
	{
	}

	public DbSet<Customer> Customers { get; set; } = null!;
	public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Customer configuration
		modelBuilder.Entity<Customer>(entity =>
		{
			entity.HasIndex(e => e.Email);
			entity.HasIndex(e => e.PhoneNumber);
		});

		// SupportTicket configuration
		modelBuilder.Entity<SupportTicket>(entity =>
		{
			entity.HasOne(t => t.User)
				.WithMany()
				.HasForeignKey(t => t.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasIndex(e => e.Status);
			entity.HasIndex(e => e.CreatedAt);
		});
	}
}

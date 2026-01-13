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
	public DbSet<Ticket> Tickets { get; set; } = null!;
	public DbSet<TicketEntry> TicketEntries { get; set; } = null!;
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

		// Ticket configuration
		modelBuilder.Entity<Ticket>(entity =>
		{
			entity.HasOne(t => t.Customer)
				.WithMany()
				.HasForeignKey(t => t.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(t => t.SystemUser)
				.WithMany()
				.HasForeignKey(t => t.SystemUserId)
				.OnDelete(DeleteBehavior.SetNull);

			entity.HasIndex(e => e.Status);
			entity.HasIndex(e => e.CreatedAt);
		});

		// TicketEntry configuration
		modelBuilder.Entity<TicketEntry>(entity =>
		{
			entity.HasOne(e => e.Ticket)
				.WithMany(t => t.Entries)
				.HasForeignKey(e => e.TicketId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.SystemUser)
				.WithMany()
				.HasForeignKey(e => e.SystemUserId)
				.OnDelete(DeleteBehavior.SetNull);
		});

		// Note: Seed data is handled by DbContextInit.SeedAsync() at runtime
	}
}

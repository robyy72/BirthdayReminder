#region Usings
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
#endregion

namespace Common;

/// <summary>
/// Aim: Factory for creating CoreDbContext at design time (migrations).
/// </summary>
public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
	/// <summary>
	/// Aim: Create a new CoreDbContext for EF migrations.
	/// Params: args - command line arguments.
	/// Return (CoreDbContext): Configured database context.
	/// </summary>
	public CoreDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();

		// Default connection string for development
		var connectionString = "Server=.\\SQLEXPRESS;Database=BirthdayReminder;Trusted_Connection=True;TrustServerCertificate=True;";

		optionsBuilder.UseSqlServer(connectionString);

		var context = new CoreDbContext(optionsBuilder.Options);
		return context;
	}
}

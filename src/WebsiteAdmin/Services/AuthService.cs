#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for admin authentication.
/// </summary>
public class AuthService
{
	#region Fields
	private readonly CoreDbContext _db;
	#endregion

	#region Constructor
	public AuthService(CoreDbContext db)
	{
		_db = db;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Validate admin credentials.
	/// Params: email - admin email, password - plain text password.
	/// Return: SystemUser if valid, null if invalid.
	/// </summary>
	public async Task<SystemUser?> ValidateCredentialsAsync(string email, string password)
	{
		var user = await _db.SystemUsers
			.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

		if (user == null)
		{
			return null;
		}

		var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
		if (!isValid)
		{
			return null;
		}

		// Update last login
		user.LastLoginAt = DateTimeOffset.UtcNow;
		await _db.SaveChangesAsync();

		return user;
	}

	/// <summary>
	/// Aim: Get admin user by ID.
	/// Params: id - user ID.
	/// Return: SystemUser or null.
	/// </summary>
	public async Task<SystemUser?> GetUserByIdAsync(int id)
	{
		var user = await _db.SystemUsers.FindAsync(id);
		return user;
	}

	/// <summary>
	/// Aim: Change admin password.
	/// Params: userId - admin ID, newPassword - new plain text password.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
	{
		var user = await _db.SystemUsers.FindAsync(userId);
		if (user == null)
		{
			return false;
		}

		user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
		await _db.SaveChangesAsync();

		return true;
	}
	#endregion
}

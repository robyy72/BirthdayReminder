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

	/// <summary>
	/// Aim: Get all system users.
	/// Params: None.
	/// Return (List of SystemUser): All system users.
	/// </summary>
	public async Task<List<SystemUser>> GetAllUsersAsync()
	{
		var users = await _db.SystemUsers
			.OrderBy(u => u.Email)
			.ToListAsync();

		return users;
	}

	/// <summary>
	/// Aim: Create a new system user.
	/// Params: email, password, displayName.
	/// Return (SystemUser): Created user.
	/// </summary>
	public async Task<SystemUser> CreateUserAsync(string email, string password, string? displayName)
	{
		var user = new SystemUser
		{
			Email = email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
			DisplayName = displayName,
			IsActive = true
		};

		_db.SystemUsers.Add(user);
		await _db.SaveChangesAsync();

		return user;
	}

	/// <summary>
	/// Aim: Update system user.
	/// Params: id, email, displayName, isActive.
	/// Return (bool): True if successful.
	/// </summary>
	public async Task<bool> UpdateUserAsync(int id, string email, string? displayName, bool isActive)
	{
		var user = await _db.SystemUsers.FindAsync(id);
		if (user == null)
		{
			return false;
		}

		user.Email = email;
		user.DisplayName = displayName;
		user.IsActive = isActive;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Delete system user.
	/// Params: id - user ID.
	/// Return (bool): True if successful.
	/// </summary>
	public async Task<bool> DeleteUserAsync(int id)
	{
		var user = await _db.SystemUsers.FindAsync(id);
		if (user == null)
		{
			return false;
		}

		_db.SystemUsers.Remove(user);
		await _db.SaveChangesAsync();
		return true;
	}
	#endregion
}

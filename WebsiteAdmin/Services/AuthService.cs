#region Usings
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for admin authentication and JWT token generation.
/// </summary>
public class AuthService
{
	#region Fields
	private readonly AdminDbContext _db;
	private readonly IConfiguration _config;
	#endregion

	#region Constructor
	public AuthService(AdminDbContext db, IConfiguration config)
	{
		_db = db;
		_config = config;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Validate admin credentials and return JWT token.
	/// Params: username - admin username, password - plain text password.
	/// Return: JWT token string or null if invalid.
	/// </summary>
	public async Task<string?> LoginAsync(string username, string password)
	{
		var user = await _db.SystemUsers
			.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

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
		user.LastLoginAt = DateTime.UtcNow;
		await _db.SaveChangesAsync();

		var token = GenerateJwtToken(user);
		return token;
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

	#region Private Methods
	private string GenerateJwtToken(SystemUser user)
	{
		var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
		var jwtIssuer = _config["Jwt:Issuer"] ?? "WebsiteAdmin";
		var jwtAudience = _config["Jwt:Audience"] ?? "WebsiteAdmin";

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
		};

		var token = new JwtSecurityToken(
			issuer: jwtIssuer,
			audience: jwtAudience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(8),
			signingCredentials: credentials
		);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
		return tokenString;
	}
	#endregion
}

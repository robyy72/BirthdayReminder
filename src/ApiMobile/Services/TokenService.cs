#region Usings
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Service for generating and validating JWT tokens for mobile app.
/// </summary>
public class TokenService
{
	#region Fields
	private readonly IConfiguration _configuration;
	#endregion

	#region Constructor
	public TokenService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Generate a JWT token for a mobile user.
	/// Params: userId - user GUID.
	/// Return: JWT token string.
	/// </summary>
	public string GenerateToken(Guid userId)
	{
		var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
		var issuer = _configuration["Jwt:Issuer"] ?? "birthday-reminder.online";
		var audience = _configuration["Jwt:Audience"] ?? "birthday-reminder-mobile";
		var expiryDays = int.Parse(_configuration["Jwt:ExpiryDays"] ?? "365");

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim("userId", userId.ToString())
		};

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddDays(expiryDays),
			signingCredentials: credentials
		);

		var result = new JwtSecurityTokenHandler().WriteToken(token);
		return result;
	}

	/// <summary>
	/// Aim: Extract user ID from claims.
	/// Params: user - claims principal.
	/// Return: User GUID or null.
	/// </summary>
	public Guid? GetUserIdFromClaims(ClaimsPrincipal user)
	{
		var userIdClaim = user.FindFirst("userId")?.Value
			?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
			?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

		if (Guid.TryParse(userIdClaim, out var userId))
		{
			return userId;
		}

		return null;
	}
	#endregion
}

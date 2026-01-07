#region Usings
using Microsoft.AspNetCore.Mvc;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Authentication endpoints for mobile app.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	#region Fields
	private readonly TokenService _tokenService;
	private readonly HeartbeatService _heartbeatService;
	#endregion

	#region Constructor
	public AuthController(TokenService tokenService, HeartbeatService heartbeatService)
	{
		_tokenService = tokenService;
		_heartbeatService = heartbeatService;
	}
	#endregion

	#region Endpoints
	/// <summary>
	/// Aim: Register or login a mobile app user.
	/// Params: request - user registration data.
	/// Return: JWT token for authenticated requests.
	/// </summary>
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request)
	{
		if (request.UserId == Guid.Empty)
		{
			return BadRequest(new { error = "UserId is required" });
		}

		// Record heartbeat (creates user if not exists)
		var heartbeat = new Common.HeartbeatDto
		{
			UserId = request.UserId,
			Email = request.Email,
			PhoneNumber = request.PhoneNumber,
			Store = request.Store
		};

		await _heartbeatService.RecordHeartbeatAsync(heartbeat);

		// Generate token
		var token = _tokenService.GenerateToken(request.UserId);

		var result = new
		{
			token,
			userId = request.UserId,
			expiresIn = 365 * 24 * 60 * 60 // 1 year in seconds
		};

		return Ok(result);
	}

	/// <summary>
	/// Aim: Refresh an existing token.
	/// Params: request - refresh token request.
	/// Return: New JWT token.
	/// </summary>
	[HttpPost("refresh")]
	public IActionResult Refresh([FromBody] RefreshRequest request)
	{
		if (request.UserId == Guid.Empty)
		{
			return BadRequest(new { error = "UserId is required" });
		}

		var token = _tokenService.GenerateToken(request.UserId);

		var result = new
		{
			token,
			userId = request.UserId,
			expiresIn = 365 * 24 * 60 * 60
		};

		return Ok(result);
	}
	#endregion
}

#region Request Models
public class RegisterRequest
{
	public Guid UserId { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public Common.AppStore Store { get; set; } = Common.AppStore.Unknown;
}

public class RefreshRequest
{
	public Guid UserId { get; set; }
}
#endregion

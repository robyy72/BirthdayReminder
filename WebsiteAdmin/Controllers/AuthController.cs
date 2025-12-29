#region Usings
using Microsoft.AspNetCore.Mvc;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Controller for admin authentication.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
	#region Fields
	private readonly AuthService _authService;
	#endregion

	#region Constructor
	public AuthController(AuthService authService)
	{
		_authService = authService;
	}
	#endregion

	#region Endpoints
	/// <summary>
	/// Aim: Admin login endpoint.
	/// </summary>
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
		{
			return BadRequest("Username and password are required");
		}

		var token = await _authService.LoginAsync(request.Username, request.Password);
		if (token == null)
		{
			return Unauthorized("Invalid credentials");
		}

		return Ok(new { token });
	}
	#endregion
}

/// <summary>
/// Aim: Login request model.
/// </summary>
public class LoginRequest
{
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}

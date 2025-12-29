#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Heartbeat endpoint for tracking app usage.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HeartbeatController : ControllerBase
{
	#region Fields
	private readonly HeartbeatService _heartbeatService;
	private readonly TokenService _tokenService;
	#endregion

	#region Constructor
	public HeartbeatController(HeartbeatService heartbeatService, TokenService tokenService)
	{
		_heartbeatService = heartbeatService;
		_tokenService = tokenService;
	}
	#endregion

	#region Endpoints
	/// <summary>
	/// Aim: Record heartbeat from mobile app.
	/// Params: dto - heartbeat data.
	/// Return: Success status.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> RecordHeartbeat([FromBody] HeartbeatDto dto)
	{
		// Verify user matches token
		var tokenUserId = _tokenService.GetUserIdFromClaims(User);
		if (tokenUserId == null || tokenUserId != dto.UserId)
		{
			return Unauthorized(new { error = "User mismatch" });
		}

		var success = await _heartbeatService.RecordHeartbeatAsync(dto);

		if (success)
		{
			return Ok(new { success = true });
		}

		return BadRequest(new { error = "Failed to record heartbeat" });
	}

	/// <summary>
	/// Aim: Get current user info.
	/// Return: User details.
	/// </summary>
	[HttpGet("me")]
	public async Task<IActionResult> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User);
		if (userId == null)
		{
			return Unauthorized();
		}

		var user = await _heartbeatService.GetUserByIdAsync(userId.Value);
		if (user == null)
		{
			return NotFound();
		}

		var result = new
		{
			userId = user.Id,
			email = user.Email,
			phoneNumber = user.PhoneNumber,
			store = user.Store,
			subscription = user.Subscription,
			subscriptionValidUntil = user.SubscriptionValidUntil,
			lastHeartbeat = user.LastHeartbeat
		};

		return Ok(result);
	}
	#endregion
}

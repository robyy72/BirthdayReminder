#region Usings
using Common;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: API controller for mobile app communication.
/// </summary>
[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
	#region Fields
	private readonly SupportService _supportService;
	private readonly HeartbeatService _heartbeatService;
	private readonly SubscriptionService _subscriptionService;
	#endregion

	#region Constructor
	public ApiController(
		SupportService supportService,
		HeartbeatService heartbeatService,
		SubscriptionService subscriptionService)
	{
		_supportService = supportService;
		_heartbeatService = heartbeatService;
		_subscriptionService = subscriptionService;
	}
	#endregion

	#region Endpoints
	/// <summary>
	/// Aim: Record heartbeat from mobile app.
	/// </summary>
	[HttpPost("heartbeat")]
	public async Task<IActionResult> Heartbeat([FromBody] HeartbeatDto dto)
	{
		var result = await _heartbeatService.RecordHeartbeatAsync(dto);
		if (!result)
		{
			return BadRequest();
		}

		return Ok();
	}

	/// <summary>
	/// Aim: Create support ticket from mobile app.
	/// </summary>
	[HttpPost("support")]
	public async Task<IActionResult> CreateSupportTicket([FromBody] SupportTicketDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Message))
		{
			return BadRequest("Message is required");
		}

		var ticketId = await _supportService.CreateTicketAsync(dto);
		return Ok(new { ticketId });
	}

	/// <summary>
	/// Aim: Verify subscription status.
	/// </summary>
	[HttpPost("subscription/verify")]
	public async Task<IActionResult> VerifySubscription([FromBody] SubscriptionStatusDto dto)
	{
		var isValid = await _subscriptionService.VerifySubscriptionAsync(dto);
		return Ok(new { isValid });
	}

	/// <summary>
	/// Aim: Health check endpoint.
	/// </summary>
	[HttpGet("health")]
	public IActionResult Health()
	{
		return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
	}
	#endregion
}

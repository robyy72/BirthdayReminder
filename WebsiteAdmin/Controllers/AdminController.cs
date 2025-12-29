#region Usings
using System.Security.Claims;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Protected API controller for admin operations.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
	#region Fields
	private readonly SupportService _supportService;
	private readonly HeartbeatService _heartbeatService;
	private readonly SubscriptionService _subscriptionService;
	private readonly AuthService _authService;
	#endregion

	#region Constructor
	public AdminController(
		SupportService supportService,
		HeartbeatService heartbeatService,
		SubscriptionService subscriptionService,
		AuthService authService)
	{
		_supportService = supportService;
		_heartbeatService = heartbeatService;
		_subscriptionService = subscriptionService;
		_authService = authService;
	}
	#endregion

	#region Dashboard
	/// <summary>
	/// Aim: Get dashboard statistics.
	/// </summary>
	[HttpGet("dashboard")]
	public async Task<IActionResult> GetDashboard()
	{
		var userStats = await _heartbeatService.GetUserStatsAsync();
		var ticketStats = await _supportService.GetTicketStatsAsync();
		var subscriptionStats = await _subscriptionService.GetSubscriptionStatsAsync();

		var result = new
		{
			users = userStats,
			tickets = ticketStats,
			subscriptions = subscriptionStats
		};

		return Ok(result);
	}
	#endregion

	#region Tickets
	/// <summary>
	/// Aim: Get all tickets with optional status filter.
	/// </summary>
	[HttpGet("tickets")]
	public async Task<IActionResult> GetTickets([FromQuery] TicketStatus? status = null)
	{
		var tickets = await _supportService.GetTicketsAsync(status);
		return Ok(tickets);
	}

	/// <summary>
	/// Aim: Get ticket by ID.
	/// </summary>
	[HttpGet("tickets/{id}")]
	public async Task<IActionResult> GetTicket(int id)
	{
		var ticket = await _supportService.GetTicketByIdAsync(id);
		if (ticket == null)
		{
			return NotFound();
		}

		return Ok(ticket);
	}

	/// <summary>
	/// Aim: Update ticket status.
	/// </summary>
	[HttpPut("tickets/{id}/status")]
	public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusRequest request)
	{
		var success = await _supportService.UpdateTicketStatusAsync(id, request.Status, request.AdminNotes);
		if (!success)
		{
			return NotFound();
		}

		return Ok();
	}

	/// <summary>
	/// Aim: Assign ticket to current admin.
	/// </summary>
	[HttpPost("tickets/{id}/assign")]
	public async Task<IActionResult> AssignTicket(int id)
	{
		var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out var adminId))
		{
			return Unauthorized();
		}

		var success = await _supportService.AssignTicketAsync(id, adminId);
		if (!success)
		{
			return NotFound();
		}

		return Ok();
	}
	#endregion

	#region Users
	/// <summary>
	/// Aim: Get all app users.
	/// </summary>
	[HttpGet("users")]
	public async Task<IActionResult> GetUsers()
	{
		var users = await _heartbeatService.GetAllUsersAsync();
		return Ok(users);
	}

	/// <summary>
	/// Aim: Get user by ID.
	/// </summary>
	[HttpGet("users/{userId}")]
	public async Task<IActionResult> GetUser(Guid userId)
	{
		var user = await _heartbeatService.GetUserByIdAsync(userId);
		if (user == null)
		{
			return NotFound();
		}

		return Ok(user);
	}

	/// <summary>
	/// Aim: Update user subscription.
	/// </summary>
	[HttpPut("users/{userId}/subscription")]
	public async Task<IActionResult> UpdateSubscription(Guid userId, [FromBody] UpdateSubscriptionRequest request)
	{
		var success = await _subscriptionService.UpdateSubscriptionAsync(userId, request.Tier, request.ValidUntil);
		if (!success)
		{
			return NotFound();
		}

		return Ok();
	}
	#endregion

	#region Account
	/// <summary>
	/// Aim: Change admin password.
	/// </summary>
	[HttpPost("account/change-password")]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
	{
		var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out var adminId))
		{
			return Unauthorized();
		}

		if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
		{
			return BadRequest("Password must be at least 8 characters");
		}

		var success = await _authService.ChangePasswordAsync(adminId, request.NewPassword);
		if (!success)
		{
			return BadRequest("Failed to change password");
		}

		return Ok();
	}
	#endregion
}

#region Request Models
public class UpdateTicketStatusRequest
{
	public TicketStatus Status { get; set; }
	public string? AdminNotes { get; set; }
}

public class UpdateSubscriptionRequest
{
	public SubscriptionTier Tier { get; set; }
	public DateTime? ValidUntil { get; set; }
}

public class ChangePasswordRequest
{
	public string NewPassword { get; set; } = string.Empty;
}
#endregion

#region Usings
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Support ticket endpoints for mobile app.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupportController : ControllerBase
{
	#region Fields
	private readonly SupportService _supportService;
	private readonly TokenService _tokenService;
	#endregion

	#region Constructor
	public SupportController(SupportService supportService, TokenService tokenService)
	{
		_supportService = supportService;
		_tokenService = tokenService;
	}
	#endregion

	#region Endpoints
	/// <summary>
	/// Aim: Create a new support ticket.
	/// Params: dto - support ticket data.
	/// Return: Created ticket ID.
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> CreateTicket([FromBody] SupportTicketDto dto)
	{
		// Verify user matches token
		var tokenUserId = _tokenService.GetUserIdFromClaims(User);
		if (tokenUserId == null || tokenUserId != dto.UserId)
		{
			return Unauthorized(new { error = "User mismatch" });
		}

		if (string.IsNullOrWhiteSpace(dto.Message))
		{
			return BadRequest(new { error = "Message is required" });
		}

		var ticketId = await _supportService.CreateTicketAsync(dto);

		var result = new
		{
			success = true,
			ticketId
		};

		return Ok(result);
	}

	/// <summary>
	/// Aim: Get tickets for current user.
	/// Return: List of user's tickets.
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> GetMyTickets()
	{
		var userId = _tokenService.GetUserIdFromClaims(User);
		if (userId == null)
		{
			return Unauthorized();
		}

		var tickets = await _supportService.GetUserTicketsAsync(userId.Value);

		var result = tickets.Select(t => new
		{
			id = t.Id,
			message = t.Message,
			type = t.Type,
			status = t.Status,
			createdAt = t.CreatedAt,
			updatedAt = t.UpdatedAt,
			closedAt = t.ClosedAt
		});

		return Ok(result);
	}
	#endregion
}

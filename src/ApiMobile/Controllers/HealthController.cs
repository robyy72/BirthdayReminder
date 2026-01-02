#region Usings
using Microsoft.AspNetCore.Mvc;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Health check endpoint for API status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
	/// <summary>
	/// Aim: Check API health status.
	/// Return: Health status object.
	/// </summary>
	[HttpGet]
	public IActionResult Get()
	{
		var result = new
		{
			status = "healthy",
			timestamp = DateTime.UtcNow,
			version = "1.0.0"
		};

		return Ok(result);
	}
}

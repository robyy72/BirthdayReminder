namespace Common;

/// <summary>
/// Aim: Model to store error information for logging and offline storage.
/// </summary>
public class ErrorModel
{
	public string App { get; set; } = string.Empty;
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
	public string ErrorMessage { get; set; } = string.Empty;
	public int HttpCode { get; set; }
	public string? StackTrace { get; set; }
	public string? Endpoint { get; set; }
	public string? UserId { get; set; }
	public string? DeviceInfo { get; set; }
	public string? AppVersion { get; set; }
}

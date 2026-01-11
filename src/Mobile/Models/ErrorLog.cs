namespace Mobile;

/// <summary>
/// Aim: Represents an error log entry stored in Prefs for offline-first error tracking.
/// </summary>
public class ErrorLog
{
	#region Properties
	public int Id { get; set; }
	public string Message { get; set; } = string.Empty;
	public string? StackTrace { get; set; }
	public string Caller { get; set; } = string.Empty;
	public string DeviceModel { get; set; } = string.Empty;
	public string AppVersion { get; set; } = string.Empty;
	public string OsVersion { get; set; } = string.Empty;
	public DateTime Timestamp { get; set; }
	public bool IsSynced { get; set; }
	public bool IsFatal { get; set; }
	#endregion
}

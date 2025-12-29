namespace Common;

/// <summary>
/// Aim: DTO for recording app heartbeat (user activity).
/// </summary>
public class HeartbeatDto
{
	public Guid UserId { get; set; }
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public string? PurchaseToken { get; set; }
	public Store Store { get; set; } = Store.Unknown;
	public PreferredChannel PreferredChannel { get; set; } = PreferredChannel.NotSet;
}

namespace Common;

/// <summary>
/// Aim: DTO for creating a support ticket from the mobile app.
/// </summary>
public class SupportTicketDto
{
	public Guid UserId { get; set; }
	public string Message { get; set; } = string.Empty;
	public string? Email { get; set; }
	public string? PhoneNumber { get; set; }
	public string? PurchaseToken { get; set; }
	public AppStore Store { get; set; } = AppStore.Unknown;
	public TicketType Type { get; set; } = TicketType.SupportRequest;
	public TicketSource Source { get; set; } = TicketSource.FromCustomerViaApp;
}

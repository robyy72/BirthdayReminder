namespace Common;

/// <summary>
/// Aim: DTO for subscription status from store APIs.
/// </summary>
public class SubscriptionStatusDto
{
	public AppStore Store { get; set; }
	public string ProductId { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public DateTime? PurchaseDate { get; set; }
	public DateTime? ExpiresAt { get; set; }
	public bool AutoRenewing { get; set; }
}

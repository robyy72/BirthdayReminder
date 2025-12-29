#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace Common;

/// <summary>
/// Aim: Customer entity for tracking mobile app users.
/// </summary>
public class Customer
{
	[Key]
	public Guid Id { get; set; }

	[MaxLength(200)]
	public string? Email { get; set; }

	[MaxLength(50)]
	public string? PhoneNumber { get; set; }

	[MaxLength(500)]
	public string? PurchaseToken { get; set; }

	public Store Store { get; set; } = Store.Unknown;

	public PreferredChannel PreferredChannel { get; set; } = PreferredChannel.NotSet;

	public SubscriptionTier Subscription { get; set; } = SubscriptionTier.Free;

	public DateTime? SubscriptionValidUntil { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;
}

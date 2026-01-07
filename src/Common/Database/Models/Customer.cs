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

	public AppStore Store { get; set; } = AppStore.Unknown;

	public PreferredChannel PreferredChannel { get; set; } = PreferredChannel.NotSet;

	public SubscriptionTier Subscription { get; set; } = SubscriptionTier.Free;

	public DateTimeOffset? SubscriptionValidUntil { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset LastHeartbeat { get; set; } = DateTimeOffset.UtcNow;
}

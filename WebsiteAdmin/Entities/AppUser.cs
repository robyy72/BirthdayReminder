#region Usings
using System.ComponentModel.DataAnnotations;
using Common;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: App user entity for tracking mobile app users.
/// </summary>
public class AppUser
{
	[Key]
	public Guid UserId { get; set; }

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

	public ICollection<SupportTicket> SupportTickets { get; set; } = [];
}

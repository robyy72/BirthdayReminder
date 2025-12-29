#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for verifying and managing subscriptions from app stores.
/// </summary>
public class SubscriptionService
{
	#region Fields
	private readonly CoreDbContext _db;
	private readonly IConfiguration _config;
	private readonly HttpClient _httpClient;
	#endregion

	#region Constructor
	public SubscriptionService(CoreDbContext db, IConfiguration config, IHttpClientFactory httpClientFactory)
	{
		_db = db;
		_config = config;
		_httpClient = httpClientFactory.CreateClient();
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Verify subscription status from store.
	/// Params: dto - subscription status data from app.
	/// Return: True if subscription is valid.
	/// </summary>
	public async Task<bool> VerifySubscriptionAsync(SubscriptionStatusDto dto)
	{
		// TODO: Implement actual store API verification
		// For now, trust the app data and update the user

		var user = await _db.Customers.FirstOrDefaultAsync(u => u.PurchaseToken == dto.ProductId);
		if (user == null)
		{
			return false;
		}

		// Update subscription info
		user.SubscriptionValidUntil = dto.ExpiresAt;

		// Determine tier from product ID
		var tier = DetermineSubscriptionTier(dto.ProductId);
		user.Subscription = tier;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Update user subscription from heartbeat/purchase.
	/// Params: userId - user ID, tier - subscription tier, validUntil - expiration date.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> UpdateSubscriptionAsync(Guid userId, SubscriptionTier tier, DateTime? validUntil)
	{
		var user = await _db.Customers.FindAsync(userId);
		if (user == null)
		{
			return false;
		}

		user.Subscription = tier;
		user.SubscriptionValidUntil = validUntil;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Check if user has active subscription.
	/// Params: userId - user ID.
	/// Return: True if subscription is active.
	/// </summary>
	public async Task<bool> IsSubscriptionActiveAsync(Guid userId)
	{
		var user = await _db.Customers.FindAsync(userId);
		if (user == null)
		{
			return false;
		}

		if (user.Subscription == SubscriptionTier.Free)
		{
			return false;
		}

		if (user.SubscriptionValidUntil.HasValue && user.SubscriptionValidUntil.Value < DateTimeOffset.UtcNow)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Aim: Get subscription statistics.
	/// Return: Subscription statistics.
	/// </summary>
	public async Task<SubscriptionStats> GetSubscriptionStatsAsync()
	{
		var stats = new SubscriptionStats
		{
			FreeUsers = await _db.Customers.CountAsync(u => u.Subscription == SubscriptionTier.Free),
			PlusUsers = await _db.Customers.CountAsync(u => u.Subscription == SubscriptionTier.Plus),
			ProUsers = await _db.Customers.CountAsync(u => u.Subscription == SubscriptionTier.Pro),
			ExpiredSubscriptions = await _db.Customers.CountAsync(u =>
				u.Subscription != SubscriptionTier.Free &&
				u.SubscriptionValidUntil.HasValue &&
				u.SubscriptionValidUntil.Value < DateTimeOffset.UtcNow)
		};

		return stats;
	}
	#endregion

	#region Private Methods
	private SubscriptionTier DetermineSubscriptionTier(string productId)
	{
		// Match product IDs to tiers
		if (productId.Contains("pro", StringComparison.OrdinalIgnoreCase))
		{
			return SubscriptionTier.Pro;
		}
		if (productId.Contains("plus", StringComparison.OrdinalIgnoreCase))
		{
			return SubscriptionTier.Plus;
		}

		return SubscriptionTier.Free;
	}
	#endregion
}

/// <summary>
/// Aim: Subscription statistics model.
/// </summary>
public class SubscriptionStats
{
	public int FreeUsers { get; set; }
	public int PlusUsers { get; set; }
	public int ProUsers { get; set; }
	public int ExpiredSubscriptions { get; set; }
}

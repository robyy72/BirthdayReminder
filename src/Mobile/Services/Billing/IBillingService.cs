namespace Mobile;

/// <summary>
/// Aim: Interface for in-app billing operations (subscriptions).
/// </summary>
public interface IBillingService
{
	/// <summary>
	/// Aim: Gets available subscription products from the store.
	/// Return (List&lt;ProductInfo&gt;): List of available products with prices.
	/// </summary>
	Task<List<ProductInfo>> GetProductsAsync();

	/// <summary>
	/// Aim: Initiates a purchase flow for the specified product.
	/// Params: productId (string) - The store product identifier.
	/// Return (PurchaseResult): Result of the purchase attempt.
	/// </summary>
	Task<PurchaseResult> PurchaseAsync(string productId);

	/// <summary>
	/// Aim: Checks if the user has an active subscription.
	/// Return (bool): True if user has active subscription, false otherwise.
	/// </summary>
	Task<bool> IsSubscribedAsync();
}

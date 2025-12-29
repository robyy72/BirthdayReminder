namespace Mobile;

/// <summary>
/// Aim: Fake billing service for development/testing (always returns success).
/// </summary>
public class FakeBillingService : IBillingService
{
	#region Private Fields
	private bool _isSubscribed = false;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Returns fake product list for testing.
	/// Return (List&lt;ProductInfo&gt;): Hardcoded test products.
	/// </summary>
	public Task<List<ProductInfo>> GetProductsAsync()
	{
		var products = new List<ProductInfo>
		{
			new("abo_monthly", "1,00 €"),
			new("abo_yearly", "10,00 €")
		};

		return Task.FromResult(products);
	}

	/// <summary>
	/// Aim: Simulates a successful purchase.
	/// Params: productId (string) - The product identifier.
	/// Return (PurchaseResult): Always returns Success.
	/// </summary>
	public Task<PurchaseResult> PurchaseAsync(string productId)
	{
		_isSubscribed = true;
		return Task.FromResult(PurchaseResult.Success);
	}

	/// <summary>
	/// Aim: Returns current subscription status.
	/// Return (bool): Current subscription state.
	/// </summary>
	public Task<bool> IsSubscribedAsync()
	{
		return Task.FromResult(_isSubscribed);
	}
	#endregion
}

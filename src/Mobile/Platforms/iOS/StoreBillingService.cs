#region Usings
using Foundation;
using StoreKit;
#endregion

#pragma warning disable CA1422 // StoreKit 1 APIs deprecated but still functional, StoreKit 2 lacks C# bindings

namespace Mobile;

/// <summary>
/// Aim: iOS billing service using StoreKit.
/// </summary>
public class StoreBillingService : NSObject, IBillingService, ISKProductsRequestDelegate, ISKPaymentTransactionObserver
{
	#region Constants
	private static readonly string[] ProductIds = ["abo_monthly", "abo_yearly"];
	#endregion

	#region Private Fields
	private TaskCompletionSource<List<SKProduct>>? _productsTcs;
	private TaskCompletionSource<PurchaseResult>? _purchaseTcs;
	private List<SKProduct> _cachedProducts = [];
	#endregion

	#region Constructor
	public StoreBillingService()
	{
		SKPaymentQueue.DefaultQueue.AddTransactionObserver(this);
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Gets available subscription products from App Store.
	/// Return (List&lt;ProductInfo&gt;): List of products with localized prices.
	/// </summary>
	public async Task<List<ProductInfo>> GetProductsAsync()
	{
		var products = new List<ProductInfo>();

		if (!SKPaymentQueue.CanMakePayments)
			return products;

		_productsTcs = new TaskCompletionSource<List<SKProduct>>();

		var productIdentifiers = new NSSet(ProductIds.Select(id => new NSString(id)).ToArray());
		var request = new SKProductsRequest(productIdentifiers);
		request.Delegate = this;
		request.Start();

		var skProducts = await _productsTcs.Task;
		_cachedProducts = skProducts;

		foreach (var skProduct in skProducts)
		{
			var formatter = new NSNumberFormatter
			{
				NumberStyle = NSNumberFormatterStyle.Currency,
				Locale = skProduct.PriceLocale
			};
			var price = formatter.StringFromNumber(skProduct.Price) ?? "N/A";

			products.Add(new ProductInfo(skProduct.ProductIdentifier, price));
		}

		return products;
	}

	/// <summary>
	/// Aim: Initiates a purchase flow for the specified subscription.
	/// Params: productId (string) - The App Store product identifier.
	/// Return (PurchaseResult): Result of the purchase attempt.
	/// </summary>
	public async Task<PurchaseResult> PurchaseAsync(string productId)
	{
		if (!SKPaymentQueue.CanMakePayments)
			return PurchaseResult.Error;

		var product = _cachedProducts.FirstOrDefault(p => p.ProductIdentifier == productId);

		if (product == null)
		{
			await GetProductsAsync();
			product = _cachedProducts.FirstOrDefault(p => p.ProductIdentifier == productId);
		}

		if (product == null)
			return PurchaseResult.Error;

		_purchaseTcs = new TaskCompletionSource<PurchaseResult>();

		var payment = SKPayment.CreateFrom(product);
		SKPaymentQueue.DefaultQueue.AddPayment(payment);

		var result = await _purchaseTcs.Task;
		return result;
	}

	/// <summary>
	/// Aim: Checks if the user has an active subscription.
	/// Return (bool): True if user has active subscription.
	/// </summary>
	public Task<bool> IsSubscribedAsync()
	{
		// On iOS, subscription status should be verified via receipt validation
		// For now, check if there are any completed subscription transactions
		var transactions = SKPaymentQueue.DefaultQueue.Transactions;

		if (transactions == null)
			return Task.FromResult(false);

		foreach (var transaction in transactions)
		{
			if (transaction.TransactionState == SKPaymentTransactionState.Purchased &&
				ProductIds.Contains(transaction.Payment?.ProductIdentifier))
			{
				return Task.FromResult(true);
			}
		}

		return Task.FromResult(false);
	}
	#endregion

	#region ISKProductsRequestDelegate
	[Export("productsRequest:didReceiveResponse:")]
	public void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
	{
		var products = response.Products?.ToList() ?? [];
		_productsTcs?.TrySetResult(products);
	}

	[Export("request:didFailWithError:")]
	public void RequestFailed(SKRequest request, NSError error)
	{
		_productsTcs?.TrySetResult([]);
	}
	#endregion

	#region ISKPaymentTransactionObserver
	[Export("paymentQueue:updatedTransactions:")]
	public void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
	{
		foreach (var transaction in transactions)
		{
			switch (transaction.TransactionState)
			{
				case SKPaymentTransactionState.Purchased:
					SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
					_purchaseTcs?.TrySetResult(PurchaseResult.Success);
					break;

				case SKPaymentTransactionState.Failed:
					SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
					var error = transaction.Error;
					if (error?.Code == 2) // User cancelled
						_purchaseTcs?.TrySetResult(PurchaseResult.Cancelled);
					else
						_purchaseTcs?.TrySetResult(PurchaseResult.Error);
					break;

				case SKPaymentTransactionState.Restored:
					SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
					_purchaseTcs?.TrySetResult(PurchaseResult.Success);
					break;

				case SKPaymentTransactionState.Deferred:
				case SKPaymentTransactionState.Purchasing:
					// Still in progress
					break;
			}
		}
	}
	#endregion

	#region Cleanup
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(this);
		}
		base.Dispose(disposing);
	}
	#endregion
}

#pragma warning restore CA1422

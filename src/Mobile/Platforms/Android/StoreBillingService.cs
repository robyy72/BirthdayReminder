#region Usings
using Android.BillingClient.Api;
using AndroidBillingResult = Android.BillingClient.Api.BillingResult;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Android billing service using Google Play Billing Library v7.
/// </summary>
public class StoreBillingService : Java.Lang.Object, IBillingService, IPurchasesUpdatedListener
{
	#region Constants
	private static readonly string[] ProductIds = ["abo_monthly", "abo_yearly"];
	#endregion

	#region Private Fields
	private BillingClient? _billingClient;
	private TaskCompletionSource<PurchaseResult>? _purchaseTcs;
	private IList<ProductDetails>? _cachedProductDetails;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Gets available subscription products from Google Play.
	/// Return (List&lt;ProductInfo&gt;): List of products with localized prices.
	/// </summary>
	public async Task<List<ProductInfo>> GetProductsAsync()
	{
		var products = new List<ProductInfo>();

		var connected = await ConnectAsync();
		if (!connected)
			return products;

		var productDetailsList = await QueryProductDetailsInternalAsync();
		_cachedProductDetails = productDetailsList;

		foreach (var details in productDetailsList)
		{
			var offerDetails = details.GetSubscriptionOfferDetails()?.FirstOrDefault();
			var pricingPhase = offerDetails?.PricingPhases?.PricingPhaseList?.FirstOrDefault();
			var price = pricingPhase?.FormattedPrice ?? "N/A";

			products.Add(new ProductInfo(details.ProductId, price));
		}

		return products;
	}

	/// <summary>
	/// Aim: Initiates a purchase flow for the specified subscription.
	/// Params: productId (string) - The Google Play product identifier.
	/// Return (PurchaseResult): Result of the purchase attempt.
	/// </summary>
	public async Task<PurchaseResult> PurchaseAsync(string productId)
	{
		var connected = await ConnectAsync();
		if (!connected)
			return PurchaseResult.Error;

		if (_cachedProductDetails == null || !_cachedProductDetails.Any())
		{
			_cachedProductDetails = await QueryProductDetailsInternalAsync();
		}

		var productDetails = _cachedProductDetails.FirstOrDefault(p => p.ProductId == productId);
		if (productDetails == null)
			return PurchaseResult.Error;

		var offerToken = productDetails.GetSubscriptionOfferDetails()?.FirstOrDefault()?.OfferToken;
		if (string.IsNullOrEmpty(offerToken))
			return PurchaseResult.Error;

		var activity = Platform.CurrentActivity;
		if (activity == null)
			return PurchaseResult.Error;

		var productDetailsParams = BillingFlowParams.ProductDetailsParams.NewBuilder()
			.SetProductDetails(productDetails)
			.SetOfferToken(offerToken)
			.Build();

		var billingFlowParams = BillingFlowParams.NewBuilder()
			.SetProductDetailsParamsList(new[] { productDetailsParams })
			.Build();

		_purchaseTcs = new TaskCompletionSource<PurchaseResult>();

		var billingResult = _billingClient!.LaunchBillingFlow(activity, billingFlowParams);

		if (billingResult.ResponseCode != BillingResponseCode.Ok)
		{
			_purchaseTcs = null;
			return PurchaseResult.Error;
		}

		var purchaseResult = await _purchaseTcs.Task;
		return purchaseResult;
	}

	/// <summary>
	/// Aim: Checks if the user has an active subscription.
	/// Return (bool): True if user has active subscription.
	/// </summary>
	public async Task<bool> IsSubscribedAsync()
	{
		var connected = await ConnectAsync();
		if (!connected)
			return false;

		var purchases = await QueryPurchasesInternalAsync();

		foreach (var purchase in purchases)
		{
			if (purchase.PurchaseState == PurchaseState.Purchased &&
				purchase.Products.Any(p => ProductIds.Contains(p)))
			{
				return true;
			}
		}

		return false;
	}
	#endregion

	#region IPurchasesUpdatedListener
	public void OnPurchasesUpdated(AndroidBillingResult billingResult, IList<Purchase>? purchases)
	{
		if (_purchaseTcs == null)
			return;

		switch (billingResult.ResponseCode)
		{
			case BillingResponseCode.Ok when purchases != null && purchases.Any():
				AcknowledgePurchases(purchases);
				_purchaseTcs.TrySetResult(PurchaseResult.Success);
				break;

			case BillingResponseCode.UserCancelled:
				_purchaseTcs.TrySetResult(PurchaseResult.Cancelled);
				break;

			case BillingResponseCode.ItemAlreadyOwned:
				_purchaseTcs.TrySetResult(PurchaseResult.AlreadyOwned);
				break;

			default:
				_purchaseTcs.TrySetResult(PurchaseResult.Error);
				break;
		}
	}
	#endregion

	#region Private Methods
	private async Task<bool> ConnectAsync()
	{
		if (_billingClient?.IsReady == true)
			return true;

		var activity = Platform.CurrentActivity;
		if (activity == null)
			return false;

		var pendingParams = PendingPurchasesParams.NewBuilder()
			.EnableOneTimeProducts()
			.EnablePrepaidPlans()
			.Build();

		_billingClient = BillingClient.NewBuilder(activity)
			.SetListener(this)
			.EnablePendingPurchases(pendingParams)
			.Build();

		var tcs = new TaskCompletionSource<bool>();

		_billingClient.StartConnection(new BillingClientStateListener(
			onConnected: () => tcs.TrySetResult(true),
			onDisconnected: () => tcs.TrySetResult(false)
		));

		return await tcs.Task;
	}

	private async Task<IList<ProductDetails>> QueryProductDetailsInternalAsync()
	{
		var productList = ProductIds
			.Select(id => QueryProductDetailsParams.Product.NewBuilder()
				.SetProductId(id)
				.SetProductType(BillingClient.ProductType.Subs)
				.Build())
			.ToList();

		var queryParams = QueryProductDetailsParams.NewBuilder()
			.SetProductList(productList)
			.Build();

		var result = await _billingClient!.QueryProductDetailsAsync(queryParams);

		if (result.Result.ResponseCode == BillingResponseCode.Ok && result.ProductDetails != null)
		{
			return result.ProductDetails;
		}

		return new List<ProductDetails>();
	}

	private async Task<IList<Purchase>> QueryPurchasesInternalAsync()
	{
		var queryParams = QueryPurchasesParams.NewBuilder()
			.SetProductType(BillingClient.ProductType.Subs)
			.Build();

		var result = await _billingClient!.QueryPurchasesAsync(queryParams);

		if (result.Result.ResponseCode == BillingResponseCode.Ok && result.Purchases != null)
		{
			return result.Purchases;
		}

		return new List<Purchase>();
	}

	private void AcknowledgePurchases(IList<Purchase> purchases)
	{
		foreach (var purchase in purchases)
		{
			if (purchase.PurchaseState == PurchaseState.Purchased && !purchase.IsAcknowledged)
			{
				var acknowledgeParams = AcknowledgePurchaseParams.NewBuilder()
					.SetPurchaseToken(purchase.PurchaseToken)
					.Build();

				_billingClient!.AcknowledgePurchase(acknowledgeParams, new AcknowledgePurchaseResponseListener());
			}
		}
	}
	#endregion
}

#region Listener Classes
internal class BillingClientStateListener : Java.Lang.Object, IBillingClientStateListener
{
	private readonly Action _onConnected;
	private readonly Action _onDisconnected;

	public BillingClientStateListener(Action onConnected, Action onDisconnected)
	{
		_onConnected = onConnected;
		_onDisconnected = onDisconnected;
	}

	public void OnBillingServiceDisconnected()
	{
		_onDisconnected();
	}

	public void OnBillingSetupFinished(AndroidBillingResult billingResult)
	{
		if (billingResult.ResponseCode == BillingResponseCode.Ok)
			_onConnected();
		else
			_onDisconnected();
	}
}

internal class AcknowledgePurchaseResponseListener : Java.Lang.Object, IAcknowledgePurchaseResponseListener
{
	public void OnAcknowledgePurchaseResponse(AndroidBillingResult billingResult)
	{
		// Purchase acknowledged - no action needed
	}
}
#endregion

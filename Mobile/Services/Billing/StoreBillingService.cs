#region Usings
using Plugin.InAppBilling;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Real billing service using Plugin.InAppBilling for store purchases.
/// </summary>
public class StoreBillingService : IBillingService
{
	#region Constants
	private static readonly string[] ProductIds = ["abo_monthly", "abo_yearly"];
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Gets available subscription products from the store.
	/// Return (List&lt;ProductInfo&gt;): List of products with localized prices.
	/// </summary>
	public async Task<List<ProductInfo>> GetProductsAsync()
	{
		var products = new List<ProductInfo>();

		try
		{
			var connected = await CrossInAppBilling.Current.ConnectAsync();
			if (!connected)
				return products;

			var items = await CrossInAppBilling.Current.GetProductInfoAsync(
				ItemType.Subscription,
				ProductIds);

			foreach (var item in items)
			{
				products.Add(new ProductInfo(item.ProductId, item.LocalizedPrice));
			}
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex);
		}
		finally
		{
			await CrossInAppBilling.Current.DisconnectAsync();
		}

		return products;
	}

	/// <summary>
	/// Aim: Initiates a purchase flow for the specified product.
	/// Params: productId (string) - The store product identifier.
	/// Return (PurchaseResult): Result of the purchase attempt.
	/// </summary>
	public async Task<PurchaseResult> PurchaseAsync(string productId)
	{
		try
		{
			var connected = await CrossInAppBilling.Current.ConnectAsync();
			if (!connected)
				return PurchaseResult.Error;

			var purchase = await CrossInAppBilling.Current.PurchaseAsync(
				productId,
				ItemType.Subscription);

			if (purchase == null)
				return PurchaseResult.Cancelled;

			// Finalize the purchase (required for subscriptions on Android)
			if (purchase.State == PurchaseState.Purchased)
			{
				await CrossInAppBilling.Current.FinalizePurchaseAsync([purchase.PurchaseToken]);
			}

			return PurchaseResult.Success;
		}
		catch (InAppBillingPurchaseException ex)
		{
			return ex.PurchaseError switch
			{
				PurchaseError.UserCancelled => PurchaseResult.Cancelled,
				PurchaseError.AlreadyOwned => PurchaseResult.AlreadyOwned,
				_ => PurchaseResult.Error
			};
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex);
			return PurchaseResult.Error;
		}
		finally
		{
			await CrossInAppBilling.Current.DisconnectAsync();
		}
	}

	/// <summary>
	/// Aim: Checks if the user has an active subscription.
	/// Return (bool): True if user has active subscription.
	/// </summary>
	public async Task<bool> IsSubscribedAsync()
	{
		try
		{
			var connected = await CrossInAppBilling.Current.ConnectAsync();
			if (!connected)
				return false;

			var purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

			foreach (var purchase in purchases)
			{
				if (ProductIds.Contains(purchase.ProductId) &&
					purchase.State == PurchaseState.Purchased)
				{
					return true;
				}
			}

			return false;
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex);
			return false;
		}
		finally
		{
			await CrossInAppBilling.Current.DisconnectAsync();
		}
	}
	#endregion
}

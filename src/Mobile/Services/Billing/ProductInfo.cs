namespace Mobile;

/// <summary>
/// Aim: Represents a product/subscription from the app store.
/// Params: ProductId - Store product identifier, LocalizedPrice - Formatted price string from store
/// </summary>
public record ProductInfo(string ProductId, string LocalizedPrice);

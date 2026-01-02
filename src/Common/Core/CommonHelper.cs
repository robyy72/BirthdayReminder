namespace Common;

/// <summary>
/// Aim: Helper methods for common operations across projects.
/// </summary>
public static class CommonHelper
{
	/// <summary>
	/// Aim: Get the API base URL based on environment.
	/// Params: isDevelopment - true for dev, false for prod.
	/// Return: API URL string.
	/// </summary>
	public static string GetApiUrl(bool isDevelopment)
	{
		var result = isDevelopment ? CommonConstants.URL_API_DEV : CommonConstants.URL_API_PROD;
		return result;
	}
}

#region Usings

using System.Text.Json;
using Common;

#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for error handling - online sends to Sentry, offline stores in prefs.
/// </summary>
public static class ErrorService
{
	/// <summary>
	/// Aim: Log an error - sends to Sentry if online, stores in prefs if offline.
	/// Params: exception - the exception to log, endpoint - optional API endpoint.
	/// </summary>
	public static void LogError(Exception exception, string? endpoint = null)
	{
		var error = CreateErrorModel(exception, endpoint);
		LogError(error);
	}

	/// <summary>
	/// Aim: Log an error - sends to Sentry if online, stores in prefs if offline.
	/// Params: errorMessage - error message, httpCode - HTTP status code, endpoint - optional API endpoint.
	/// </summary>
	public static void LogError(string errorMessage, int httpCode = 0, string? endpoint = null)
	{
		var error = new ErrorModel
		{
			App = MobileConstants.APP_NAME,
			Timestamp = DateTimeOffset.Now,
			ErrorMessage = errorMessage,
			HttpCode = httpCode,
			Endpoint = endpoint,
			DeviceInfo = GetDeviceInfo(),
			AppVersion = GetAppVersion()
		};

		LogError(error);
	}

	/// <summary>
	/// Aim: Log an ErrorModel - sends to Sentry if online, stores in prefs if offline.
	/// Params: error - the ErrorModel to log.
	/// </summary>
	public static void LogError(ErrorModel error)
	{
		if (MobileService.HasNetworkAccess())
		{
			SendToSentry(error);
			SendPendingErrors();
		}
		else
		{
			StoreErrorOffline(error);
		}
	}

	/// <summary>
	/// Aim: Send error to Sentry.
	/// Params: error - the ErrorModel to send.
	/// </summary>
	private static void SendToSentry(ErrorModel error)
	{
		// TODO: Implement Sentry integration
		// SentrySdk.CaptureMessage(error.ErrorMessage);
		// SentrySdk.CaptureException(new Exception(error.ErrorMessage));

		System.Diagnostics.Debug.WriteLine($"[Sentry] Would send error: {error.ErrorMessage}");
	}

	/// <summary>
	/// Aim: Store error in preferences for later sending.
	/// Params: error - the ErrorModel to store.
	/// </summary>
	private static void StoreErrorOffline(ErrorModel error)
	{
		var errors = GetStoredErrors();
		errors.Add(error);
		SaveErrors(errors);
	}

	/// <summary>
	/// Aim: Send all pending errors stored in preferences.
	/// </summary>
	public static void SendPendingErrors()
	{
		if (!MobileService.HasNetworkAccess())
		{
			return;
		}

		var errors = GetStoredErrors();
		if (errors.Count == 0)
		{
			return;
		}

		foreach (var error in errors)
		{
			SendToSentry(error);
		}

		ClearStoredErrors();
	}

	/// <summary>
	/// Aim: Get all stored errors from preferences.
	/// Return: List of ErrorModel objects.
	/// </summary>
	public static List<ErrorModel> GetStoredErrors()
	{
		var json = Preferences.Get(MobileConstants.PREFS_ERRORS, string.Empty);
		if (string.IsNullOrEmpty(json))
		{
			return new List<ErrorModel>();
		}

		try
		{
			return JsonSerializer.Deserialize<List<ErrorModel>>(json) ?? new List<ErrorModel>();
		}
		catch
		{
			return new List<ErrorModel>();
		}
	}

	/// <summary>
	/// Aim: Save errors list to preferences.
	/// Params: errors - list of ErrorModel objects to save.
	/// </summary>
	private static void SaveErrors(List<ErrorModel> errors)
	{
		var json = JsonSerializer.Serialize(errors);
		Preferences.Set(MobileConstants.PREFS_ERRORS, json);
	}

	/// <summary>
	/// Aim: Clear all stored errors from preferences.
	/// </summary>
	public static void ClearStoredErrors()
	{
		Preferences.Remove(MobileConstants.PREFS_ERRORS);
	}

	/// <summary>
	/// Aim: Create ErrorModel from exception.
	/// Params: exception - the exception, endpoint - optional API endpoint.
	/// Return: ErrorModel object.
	/// </summary>
	private static ErrorModel CreateErrorModel(Exception exception, string? endpoint = null)
	{
		int httpCode = 0;
		if (exception is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
		{
			httpCode = (int)httpEx.StatusCode.Value;
		}

		return new ErrorModel
		{
			App = MobileConstants.APP_NAME,
			Timestamp = DateTimeOffset.Now,
			ErrorMessage = exception.Message,
			HttpCode = httpCode,
			StackTrace = exception.StackTrace,
			Endpoint = endpoint,
			DeviceInfo = GetDeviceInfo(),
			AppVersion = GetAppVersion()
		};
	}

	/// <summary>
	/// Aim: Get device information string.
	/// Return: Device info string.
	/// </summary>
	private static string GetDeviceInfo()
	{
		return $"{DeviceInfo.Platform} {DeviceInfo.VersionString} - {DeviceInfo.Manufacturer} {DeviceInfo.Model}";
	}

	/// <summary>
	/// Aim: Get app version string.
	/// Return: App version string.
	/// </summary>
	private static string GetAppVersion()
	{
		return AppInfo.VersionString;
	}
}

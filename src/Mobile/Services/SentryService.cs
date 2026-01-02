#region Usings
using Sentry;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for Sentry error reporting integration.
/// </summary>
public static class SentryService
{
	#region Private Fields
	private static bool _isInitialized;
	#endregion

	#region Properties
	/// <summary>
	/// Aim: Check if Sentry is initialized.
	/// </summary>
	public static bool IsInitialized => _isInitialized;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Initialize Sentry with DSN from configuration.
	/// Params: dsn (string) - The Sentry DSN.
	/// </summary>
	public static void Initialize(string? dsn)
	{
		if (_isInitialized) return;
		if (string.IsNullOrWhiteSpace(dsn)) return;

		_isInitialized = true;
	}

	/// <summary>
	/// Aim: Capture an exception and send to Sentry.
	/// Params: ex (Exception) - The exception to capture.
	/// Return (bool): True if captured, false if Sentry not initialized.
	/// </summary>
	public static bool CaptureException(Exception ex)
	{
		if (!_isInitialized) return false;

		try
		{
			SentrySdk.CaptureException(ex);
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Aim: Capture a message and send to Sentry.
	/// Params: message (string) - The message to capture.
	/// Params: level (SentryLevel) - The severity level.
	/// Return (bool): True if captured, false if Sentry not initialized.
	/// </summary>
	public static bool CaptureMessage(string message, SentryLevel level = SentryLevel.Error)
	{
		if (!_isInitialized) return false;

		try
		{
			SentrySdk.CaptureMessage(message, level);
			return true;
		}
		catch
		{
			return false;
		}
	}
	#endregion
}

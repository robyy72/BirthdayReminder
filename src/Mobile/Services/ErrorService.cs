#region Usings
using System.Runtime.CompilerServices;
using Sentry;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for error handling - stores in SQLite, syncs to Sentry when online.
/// </summary>
public static class ErrorService
{
	#region Public Methods
	/// <summary>
	/// Aim: Handle a non-fatal error - stores in SQLite, sends to Sentry if online.
	/// Params: ex (Exception) - The exception to handle.
	/// Params: caller (string) - Auto-captured caller method name.
	/// </summary>
	public static async void Handle(Exception ex, [CallerMemberName] string caller = "")
	{
		var log = CreateErrorLog(ex, caller, isFatal: false);
		await ErrorDatabase.SaveAsync(log);

		if (MobileService.HasNetworkAccess() && SentryService.IsInitialized)
		{
			bool sent = SentryService.CaptureException(ex);
			if (sent)
			{
				await ErrorDatabase.MarkSyncedAsync(log.Id);
			}
		}
	}

	/// <summary>
	/// Aim: Handle a fatal error - marks version as broken, stores in SQLite, sends to Sentry.
	/// Params: ex (Exception) - The exception to handle.
	/// </summary>
	public static async void HandleFatal(Exception ex)
	{
		Preferences.Set(MobileConstants.PREFS_BROKEN_VERSION, AppInfo.VersionString);
		Preferences.Set(MobileConstants.PREFS_BROKEN_DEVICE, DeviceInfo.Model);
		Preferences.Set(MobileConstants.PREFS_BROKEN_TIMESTAMP, DateTime.UtcNow.ToString("O"));

		var log = CreateErrorLog(ex, "FATAL", isFatal: true);
		await ErrorDatabase.SaveAsync(log);

		if (SentryService.IsInitialized)
		{
			SentryService.CaptureException(ex);
		}
	}

	/// <summary>
	/// Aim: Sync all pending errors to Sentry and clean up old logs.
	/// Return (Task): Async task.
	/// </summary>
	public static async Task SyncPendingAsync()
	{
		if (!MobileService.HasNetworkAccess()) return;
		if (!SentryService.IsInitialized) return;

		var pending = await ErrorDatabase.GetUnsyncedAsync();

		foreach (var log in pending)
		{
			string level = log.IsFatal ? "FATAL" : "ERROR";
			bool sent = SentryService.CaptureMessage($"[{level}] {log.Caller}: {log.Message}", SentryLevel.Error);
			if (sent)
			{
				await ErrorDatabase.MarkSyncedAsync(log.Id);
			}
			else
			{
				break;
			}
		}

		await ErrorDatabase.DeleteOldLogsAsync(MobileConstants.ERROR_RETENTION_DAYS);
	}

	/// <summary>
	/// Aim: Check if current version is marked as broken.
	/// Return (bool): True if current version crashed before.
	/// </summary>
	public static bool IsBrokenVersion()
	{
		string brokenVersion = Preferences.Get(MobileConstants.PREFS_BROKEN_VERSION, string.Empty);
		bool isBroken = brokenVersion == AppInfo.VersionString;
		return isBroken;
	}

	/// <summary>
	/// Aim: Clear broken version marker (e.g., after update or user chose to try anyway).
	/// </summary>
	public static void ClearBrokenVersionMarker()
	{
		Preferences.Remove(MobileConstants.PREFS_BROKEN_VERSION);
		Preferences.Remove(MobileConstants.PREFS_BROKEN_DEVICE);
		Preferences.Remove(MobileConstants.PREFS_BROKEN_TIMESTAMP);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Aim: Create ErrorLog from exception.
	/// Params: ex (Exception) - The exception.
	/// Params: caller (string) - The caller method name.
	/// Params: isFatal (bool) - Whether this is a fatal error.
	/// Return (ErrorLog): The error log object.
	/// </summary>
	private static ErrorLog CreateErrorLog(Exception ex, string caller, bool isFatal)
	{
		var log = new ErrorLog
		{
			Message = ex.Message,
			StackTrace = ex.StackTrace,
			Caller = caller,
			DeviceModel = DeviceInfo.Model,
			AppVersion = AppInfo.VersionString,
			OsVersion = DeviceInfo.VersionString,
			Timestamp = DateTime.UtcNow,
			IsSynced = false,
			IsFatal = isFatal
		};
		return log;
	}
	#endregion
}

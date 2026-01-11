#region Usings
using System.Runtime.CompilerServices;
using Sentry;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for error handling - stores in Prefs, syncs to Sentry when online.
/// </summary>
public static class ErrorService
{
	#region Public Methods
	/// <summary>
	/// Aim: Load error logs from Prefs into App.ErrorLogs.
	/// </summary>
	public static void Load()
	{
		var logs = PrefsHelper.GetValue<List<ErrorLog>>(MobileConstants.PREFS_NOT_UPLOADED_ERROR_LOGS);
		if (logs != null)
		{
			App.ErrorLogs = logs;
		}
		else
		{
			App.ErrorLogs = new List<ErrorLog>();
		}
	}

	/// <summary>
	/// Aim: Save App.ErrorLogs to Prefs.
	/// </summary>
	public static void Save()
	{
		PrefsHelper.SetValue(MobileConstants.PREFS_NOT_UPLOADED_ERROR_LOGS, App.ErrorLogs);
	}

	/// <summary>
	/// Aim: Handle a non-fatal error - sends to Sentry if online, stores in Prefs if offline.
	/// Params: ex (Exception) - The exception to handle.
	/// Params: caller (string) - Auto-captured caller method name.
	/// </summary>
	public static void Handle(Exception ex, [CallerMemberName] string caller = "")
	{
		if (MobileService.HasNetworkAccess() && SentryService.IsInitialized)
		{
			SentryService.CaptureException(ex);
		}
		else
		{
			var log = CreateErrorLog(ex, caller, isFatal: false);
			AddLog(log);
		}
	}

	/// <summary>
	/// Aim: Handle a fatal error - marks version as broken, stores in Prefs, sends to Sentry.
	/// Params: ex (Exception) - The exception to handle.
	/// </summary>
	public static void HandleFatal(Exception ex)
	{
		Preferences.Set(MobileConstants.PREFS_BROKEN_VERSION, AppInfo.BuildString);
		Preferences.Set(MobileConstants.PREFS_BROKEN_DEVICE, DeviceInfo.Model);
		Preferences.Set(MobileConstants.PREFS_BROKEN_TIMESTAMP, DateTime.UtcNow.ToString("O"));

		var log = CreateErrorLog(ex, "FATAL", isFatal: true);
		AddLog(log);

		if (SentryService.IsInitialized)
		{
			SentryService.CaptureException(ex);
		}
	}

	/// <summary>
	/// Aim: Sync all pending errors to Sentry and clean up synced logs.
	/// </summary>
	public static void SyncPending()
	{
		if (!MobileService.HasNetworkAccess()) return;
		if (!SentryService.IsInitialized) return;

		var pending = App.ErrorLogs.Where(l => !l.IsSynced).ToList();

		foreach (var log in pending)
		{
			string level = log.IsFatal ? "FATAL" : "ERROR";
			bool sent = SentryService.CaptureMessage($"[{level}] {log.Caller}: {log.Message}", SentryLevel.Error);
			if (sent)
			{
				log.IsSynced = true;
			}
			else
			{
				break;
			}
		}

		// Remove synced logs
		App.ErrorLogs.RemoveAll(l => l.IsSynced);

		Save();
	}

	/// <summary>
	/// Aim: Check if current version is marked as broken.
	/// Return (bool): True if current version crashed before.
	/// </summary>
	public static bool IsBrokenVersion()
	{
		string brokenVersion = Preferences.Get(MobileConstants.PREFS_BROKEN_VERSION, string.Empty);
		bool isBroken = brokenVersion == AppInfo.BuildString;
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
	/// Aim: Add error log to App.ErrorLogs and save. Skips if limit reached.
	/// Params: log (ErrorLog) - The error log to add.
	/// </summary>
	private static void AddLog(ErrorLog log)
	{
		if (App.ErrorLogs.Count >= MobileConstants.ERROR_MAX_OFFLINE_ENTRIES)
			return;

		log.Id = GetNextId();
		App.ErrorLogs.Add(log);
		Save();
	}

	/// <summary>
	/// Aim: Get next available ID for error log.
	/// Return (int): Next ID.
	/// </summary>
	private static int GetNextId()
	{
		if (App.ErrorLogs.Count == 0)
		{
			return 1;
		}

		var maxId = App.ErrorLogs.Max(l => l.Id);
		return maxId + 1;
	}

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

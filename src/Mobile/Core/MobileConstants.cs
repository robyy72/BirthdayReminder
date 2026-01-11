namespace Mobile;

// see also Common/Core/CommonConstants.cs

public static class MobileConstants
{
	#region App Info
	public const string APP_NAME = "Mobile";
	#endregion

	#region External URLs
	public const string URL_TIMEZONE_INFO = "https://www.timeanddate.com/time/zones/";
	public const string URL_PRIVACY_POLICY = "https://birthday-reminder.online/privacy";
	public const string URL_TERMS_OF_SERVICE = "https://birthday-reminder.online/terms";
	#endregion

	#region Localization
	public static readonly string[] SUPPORTED_LOCALES = ["de", "en"];
	public const string DEFAULT_LOCALE = "de";
	#endregion

	#region Theme
	public const string DEFAULT_THEME = "System";
	#endregion

	#region Display
	public const int SHOW_UPCOMING_DEFAULT = 5;
	public const int SHOW_PAST_DEFAULT = 3;
	public const int SHOW_MAX_BIRTHDAYS = 100;
	#endregion

	#region Reminders
	public const int MAX_REMINDERS = 3;
	public const int MAX_REMINDER_DAYS = 30;
	#endregion

	#region Age
	public const int MAX_AGE = 120;
	public const string YEARS_DISPLAY_NOT = "1,1604,1900,1904";
	#endregion

	#region Debug, Errors and Sentry
	public const bool START_ALWAYS_WITH_WELCOME = false;
	public const string SENTRY_DSN = "https://a7322c361a63c2577a1afa16efe302bb@o4510638147829760.ingest.de.sentry.io/4510638162116688";
	public const int ERROR_MAX_OFFLINE_ENTRIES = 10;
	#endregion

	#region Preferences Keys
	public const string PREFS_ACCOUNT = "Account";
	public const string PREFS_ACCOUNT_INITIALIZED = "AccountInitialized";
	public const string PREFS_ACCOUNT_FIRST_RUN_COMPLETED = "AccountFirstRun";
	public const string PREFS_PERSONS = "Persons";
	public const string PREFS_REMINDER_SETTINGS_SHOWN = "ReminderSettingsShown";
	public const string PREFS_SUPPORT = "Support";
	public const string PREFS_SUPPORT_ENTRIES = "SupportEntries";
	public const string PREFS_NOT_UPLOADED_TICKETS = "NotUploadedTickets";
	public const string PREFS_NOT_UPLOADED_ERROR_LOGS = "NotUploadedErrorLogs";
	public const string PREFS_BROKEN_VERSION = "BrokenVersion";
	public const string PREFS_BROKEN_DEVICE = "BrokenDevice";
	public const string PREFS_BROKEN_TIMESTAMP = "BrokenTimestamp";
	#endregion
}


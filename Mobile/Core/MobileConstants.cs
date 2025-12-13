namespace Mobile;

public static class MobileConstants
{
	#region App Info
	public const string APP_NAME = "Mobile";
	#endregion

	#region Debug
	public const bool START_ALWAYS_WITH_WELCOME = true;
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

	#region Errors
	public const int MAX_ERRORS_INTO_PREFS = 10;
	#endregion

	#region Preferences Keys
	public const string PREFS_BIRTHDAYS_PREFIX = "Birthdays_";
	public const string PREFS_ERRORS = "Errors";
	public const string PREFS_PERSON_PREFIX = "Person_";
	public const string PREFS_SETTINGS = "Settings";
	public const string PREFS_SETTINGS_INITIALIZED = "SettingsInitialized";
	public const string PREFS_REMINDER_SETTINGS_SHOWN = "ReminderSettingsShown";
	#endregion
}

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

	#region Reminders
	public const int MAX_REMINDERS = 3;
	public const int MAX_REMINDER_DAYS = 30;
	#endregion

	#region Age
	public const int MAX_AGE = 120;
	public const string YEARS_DISPLAY_NOT = "1,1604,1900,1904";
	#endregion

	#region Errors
	public const int MAX_ERRORS_INTO_PREFS = 10;
	#endregion

	#region Preferences Keys
	public const string PREFS_ACCOUNT = "Account";
	public const string PREFS_ACCOUNT_INITIALIZED = "AccountInitialized";
	public const string PREFS_ACCOUNT_FIRST_RUN = "AccountFirstRun";
	public const string PREFS_ERRORS = "Errors";
	public const string PREFS_PERSONS = "Persons";
	public const string PREFS_REMINDER_SETTINGS_SHOWN = "ReminderSettingsShown";
	#endregion
}

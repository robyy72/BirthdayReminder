namespace Mobile;

public static class MobileConstants
{
	#region App Info
	public const string APP_NAME = "Mobile";
	#endregion

	#region Debug
	public const bool START_ALWAYS_WITH_WELCOME = true;
    #endregion

    #region Supported Values
    public static readonly string[] SUPPORTED_LOCALES = ["de", "en"];
    #endregion

    #region Defaults
    public const string DEFAULT_LOCALE = "de";
	public const string DEFAULT_THEME = "System";
	public const int DEFAULT_REMINDER_TIME = 1800;
	#endregion

	#region Display
	public const int SHOW_UPCOMING = 5;
	public const int SHOW_MISSED_BIRTHDAYS = 3;
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
	#endregion
}

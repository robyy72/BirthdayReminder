namespace Mobile;

public static class MobileConstants
{
	public const bool CLEAN_PREFS_AT_START = true;

	public const int DEFAULT_REMINDER_TIME = 1800;
	public const string DEFAULT_LOCALE = "de";
	public static readonly string[] SUPPORTED_LOCALES = ["de", "en"];

	public const int SHOW_UPCOMING = 5;
	public const int SHOW_MISSED_BIRTHDAYS = 3;

	public const string PREFS_PERSON_PREFIX = "Person_";
	public const string PREFS_BIRTHDAYS_PREFIX = "Birthdays_";
	public const string PREFS_SETTINGS = "Settings";
	public const string PREFS_ERRORS = "Errors";

	public const string APP_NAME = "Mobile";
}

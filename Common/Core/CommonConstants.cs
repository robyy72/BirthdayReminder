namespace Common;

/// <summary>
/// Aim: Common constants used across all projects.
/// </summary>
public static class CommonConstants
{
    #region Domain und Api-Base-URLs
    public const string DOMAIN = "birthday-reminder.online";
    public const string URL_API_DEV = "https://api-dev." + DOMAIN;
    public const string URL_API_PROD = "https://api-prod." + DOMAIN;
    #endregion

    #region Lengths / Min- and Max
    public const int MAX_LENGTH_DISPLAYNAME = 50;
    #endregion

	#region Reminder Times
	public const int DEFAULT_REMINDER_TIME_MORNING = 900;
	public const int DEFAULT_REMINDER_TIME_EVENING = 1800;
	public const int DEFAULT_REMINDER_TIME_EMAIL = DEFAULT_REMINDER_TIME_MORNING;
	public const int DEFAULT_REMINDER_TIME_SMS = DEFAULT_REMINDER_TIME_EVENING;
	public const int DEFAULT_REMINDER_TIME_LOCKSCREEN = DEFAULT_REMINDER_TIME_MORNING;
	public const int DEFAULT_REMINDER_TIME_WHATSAPP = DEFAULT_REMINDER_TIME_EVENING;
	public const int DEFAULT_REMINDER_TIME_SIGNAL = DEFAULT_REMINDER_TIME_EVENING;
	#endregion

	#region Reminder Defaults - Local
	public const bool DEFAULT_NOTIFICATION_SOUND = true;
	public const bool DEFAULT_NOTIFICATION_VIBRATE = false;
	public const bool DEFAULT_NOTIFICATION_WAKE_SCREEN = false;
	public const bool DEFAULT_NOTIFICATION_OVERRIDE_SILENT = false;

	public const bool DEFAULT_ALARM_SOUND = true;
	public const bool DEFAULT_ALARM_VIBRATE = true;
	public const bool DEFAULT_ALARM_WAKE_SCREEN = true;
	public const bool DEFAULT_ALARM_OVERRIDE_SILENT = false;
	#endregion

	#region Reminder Defaults - External
	public const bool DEFAULT_MESSAGE_INCLUDE_AGE = false;
	#endregion
}

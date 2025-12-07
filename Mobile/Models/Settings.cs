namespace Mobile;

/// <summary>
/// Aim: Represents application settings.
/// </summary>
public class Settings
{
	public BirthdaySource BirthdaySource { get; set; } = BirthdaySource.NotSet;
	public int DefaultReminderTime { get; set; } = MobileConstants.DEFAULT_REMINDER_TIME;
	public string Locale { get; set; } = MobileConstants.DEFAULT_LOCALE;
}

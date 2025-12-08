namespace Mobile;

/// <summary>
/// Aim: Represents application settings.
/// </summary>
public class Settings
{
	public bool ReadFromContactsWithBirthday { get; set; }
	public bool ReadFromContactsAll { get; set; }
	public bool WriteToContacts { get; set; }
	public bool ReadFromCalenders { get; set; }
	public bool WriteToCalenders { get; set; }

	// List of Calenders which the User gave access to

	public int DefaultReminderTime { get; set; } = MobileConstants.DEFAULT_REMINDER_TIME;
	public string Locale { get; set; } = MobileConstants.DEFAULT_LOCALE;
}

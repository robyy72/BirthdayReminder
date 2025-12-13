namespace Mobile;

#region Usings
using Common;
#endregion

/// <summary>
/// Aim: Represents application settings.
/// </summary>
public class Settings
{
	public ContactsMode ContactsMode { get; set; } = ContactsMode.None;
	public bool WriteToCalendars { get; set; }
	public List<string> SelectedCalendarIds { get; set; } = [];
	public int DefaultReminderTime { get; set; } = MobileConstants.DEFAULT_REMINDER_TIME;
	public string Locale { get; set; } = MobileConstants.DEFAULT_LOCALE;
	public string Theme { get; set; } = MobileConstants.DEFAULT_THEME;
	public PersonNameDirection PersonNameDirection { get; set; } = PersonNameDirection.FirstFirstName;
	public int ShowUpcomingBirthdays { get; set; } = MobileConstants.SHOW_UPCOMING_DEFAULT;
	public int ShowPastBirthdays { get; set; } = MobileConstants.SHOW_PAST_DEFAULT;
	public bool RemindUntilApproved { get; set; }
}

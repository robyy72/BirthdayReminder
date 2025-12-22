#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents application account/settings.
/// </summary>
public class Account
{
	#region Display
	public string Locale { get; set; } = MobileConstants.DEFAULT_LOCALE;
	public string Theme { get; set; } = MobileConstants.DEFAULT_THEME;
	public PersonNameDirection PersonNameDirection { get; set; } = PersonNameDirection.FirstFirstName;
	public int ShowUpcomingBirthdays { get; set; } = MobileConstants.SHOW_UPCOMING_DEFAULT;
	public int ShowPastBirthdays { get; set; } = MobileConstants.SHOW_PAST_DEFAULT;
    #endregion

    #region Reminder
    public ReminderCount ReminderCount { get; set; } = ReminderCount.NoReminder;
	#endregion

	#region Contacts
	public ContactsMode ContactsMode { get; set; } = ContactsMode.None;
	#endregion

	#region Calendars
	public bool WriteToCalendars { get; set; }
	public List<string> SelectedCalendarIds { get; set; } = [];
	#endregion
}

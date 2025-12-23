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
	public ContactsReadMode ContactsReadMode { get; set; } = ContactsReadMode.None;
	public ContactsReadWriteMode ContactsReadWriteMode { get; set; } = ContactsReadWriteMode.NotSet;
	public ContactsAccessChoice ContactsAccessChoice { get; set; } = ContactsAccessChoice.NotSet;
	#endregion

	#region Calendars
	// Device Calendar
	public bool DeviceCalendar_Enabled { get; set; }
	public List<string> DeviceCalendar_SelectedIds { get; set; } = [];
	// Outlook Calendar
	public bool OutlookCalendar_SingleEmail { get; set; }
	public bool OutlookCalendar_GrantAccess { get; set; }
	// Google Calendar
	public bool GoogleCalendar_SingleEmail { get; set; }
	public bool GoogleCalendar_GrantAccess { get; set; }
	#endregion
}

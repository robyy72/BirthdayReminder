#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents application settings.
/// </summary>
public class Settings
{
	#region Display
	public string Locale { get; set; } = MobileConstants.DEFAULT_LOCALE;
	public string Theme { get; set; } = MobileConstants.DEFAULT_THEME;
	public PersonNameDirection PersonNameDirection { get; set; } = PersonNameDirection.FirstFirstName;
	public int ShowUpcomingBirthdays { get; set; } = MobileConstants.SHOW_UPCOMING_DEFAULT;
	public int ShowPastBirthdays { get; set; } = MobileConstants.SHOW_PAST_DEFAULT;
    #endregion

    #region Reminder Enabled and Time-Values
    public bool ReminderEmailEnabled { get; set; }
    public bool ReminderSmsEnabled { get; set; }
    public bool ReminderLockScreenEnabled { get; set; }
    public bool ReminderWhatsAppEnabled { get; set; }
    
	public int ReminderTimeEmail { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_EMAIL;
	public int ReminderTimeSms { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_SMS;
	public int ReminderTimeLockScreen { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_LOCKSCREEN;
	public int ReminderTimeWhatsApp { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_WHATSAPP;

	public bool RemindUntilApproved { get; set; }
	#endregion

	#region Contacts
	public ContactsMode ContactsMode { get; set; } = ContactsMode.None;
	#endregion

	#region Calendars
	public bool WriteToCalendars { get; set; }
	public List<string> SelectedCalendarIds { get; set; } = [];
	#endregion
}

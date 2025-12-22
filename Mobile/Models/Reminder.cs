#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a reminder configuration with method flags, times and days before birthday.
/// </summary>
public class Reminder
{
	#region Methods Enabled
	public bool EmailEnabled { get; set; }
	public bool SmsEnabled { get; set; }
	public bool LockScreenEnabled { get; set; }
	public bool WhatsAppEnabled { get; set; }
	public bool SignalEnabled { get; set; }
	#endregion

	#region Method Times
	public int TimeEmail { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_EMAIL;
	public int TimeSms { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_SMS;
	public int TimeLockScreen { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_LOCKSCREEN;
	public int TimeWhatsApp { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_WHATSAPP;
	public int TimeSignal { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_SIGNAL;
	#endregion

	#region Days Before
	public int Days { get; set; }
	#endregion
}

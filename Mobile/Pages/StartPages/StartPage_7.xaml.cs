#region Usings
using Common;
#endregion

namespace Mobile;

public partial class StartPage_7 : ContentPage
{
	public StartPage_7()
	{
		InitializeComponent();
	}

	private Reminder CreateReminder()
	{
		int.TryParse(DaysEntry.Text, out int days);

		var reminder = new Reminder
		{
			DaysBefore = days,
			LocalMethods =
			[
				new ReminderMethodLocal
				{
					Type = LocalMethodType.Notification,
					Enabled = NotificationSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.Local[LocalMethodType.Notification].DefaultTime,
					PlaySound = CommonConstants.DEFAULT_NOTIFICATION_SOUND,
					Vibrate = CommonConstants.DEFAULT_NOTIFICATION_VIBRATE,
					WakeScreen = CommonConstants.DEFAULT_NOTIFICATION_WAKE_SCREEN,
					OverrideSilentMode = CommonConstants.DEFAULT_NOTIFICATION_OVERRIDE_SILENT,
					Priority = NotificationPriority.Normal
				},
				new ReminderMethodLocal
				{
					Type = LocalMethodType.Alarm,
					Enabled = AlarmSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.Local[LocalMethodType.Alarm].DefaultTime,
					PlaySound = CommonConstants.DEFAULT_ALARM_SOUND,
					Vibrate = CommonConstants.DEFAULT_ALARM_VIBRATE,
					WakeScreen = CommonConstants.DEFAULT_ALARM_WAKE_SCREEN,
					OverrideSilentMode = CommonConstants.DEFAULT_ALARM_OVERRIDE_SILENT,
					Priority = NotificationPriority.High
				}
			],
			ExternalMethods =
			[
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.Email,
					Enabled = EmailSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.External[ExternalMethodType.Email].DefaultTime,
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.Sms,
					Enabled = SmsSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.External[ExternalMethodType.Sms].DefaultTime,
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.WhatsApp,
					Enabled = WhatsAppSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.External[ExternalMethodType.WhatsApp].DefaultTime,
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.Signal,
					Enabled = SignalSwitch.IsToggled,
					TimeMinutes = ReminderMethodConfig.External[ExternalMethodType.Signal].DefaultTime,
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				}
			]
		};

		return reminder;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_6());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.Reminder_2_Template = CreateReminder();

		if (App.Account.ReminderCount == ReminderCount.ThreeReminders)
			App.SetRootPage(new StartPage_8());
		else
			App.SetRootPage(new StartPage_9());
	}
}

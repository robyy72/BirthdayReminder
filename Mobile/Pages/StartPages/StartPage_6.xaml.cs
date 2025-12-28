#region Usings
using Common;
#endregion

namespace Mobile;

public partial class StartPage_6 : ContentPage
{
	public StartPage_6()
	{
		InitializeComponent();
		LoadReminder();
	}

	private void LoadReminder()
	{
		var reminder = App.Reminder_1_Template;
		if (reminder != null)
		{
			DaysEntry.Text = reminder.DaysBefore.ToString();

			// Local methods
			var notification = reminder.LocalMethods.FirstOrDefault(m => m.Type == LocalMethodType.Notification);
			var alarm = reminder.LocalMethods.FirstOrDefault(m => m.Type == LocalMethodType.Alarm);
			NotificationSwitch.IsToggled = notification?.Enabled ?? false;
			AlarmSwitch.IsToggled = alarm?.Enabled ?? false;

			// External methods
			var email = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Email);
			var sms = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Sms);
			var whatsApp = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.WhatsApp);
			var signal = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Signal);
			EmailSwitch.IsToggled = email?.Enabled ?? false;
			SmsSwitch.IsToggled = sms?.Enabled ?? false;
			WhatsAppSwitch.IsToggled = whatsApp?.Enabled ?? false;
			SignalSwitch.IsToggled = signal?.Enabled ?? false;
		}
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
		App.SetRootPage(new StartPage_5());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.Reminder_1_Template = CreateReminder();

		if (App.Account.ReminderCount >= ReminderCount.TwoReminders)
			App.SetRootPage(new StartPage_7());
		else
			App.SetRootPage(new StartPage_9());
	}
}

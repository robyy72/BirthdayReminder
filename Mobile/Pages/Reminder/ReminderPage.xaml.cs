#region Usings
using Common;
#endregion

namespace Mobile;

public partial class ReminderPage : ContentPage
{
	#region Private Fields
	private readonly int _reminderIndex;
	private readonly Person? _person;
	private readonly bool _isWizardMode;
	private List<int> _availableDays = [];
	private int _currentDays = 0;
	#endregion

	#region Constructor
	/// <summary>
	/// Aim: Creates a ReminderPage for configuring a specific reminder (index 0, 1, or 2).
	/// Params: reminderIndex - The reminder index (0, 1, or 2)
	/// </summary>
	public ReminderPage(int reminderIndex) : this(reminderIndex, null)
	{
	}

	/// <summary>
	/// Aim: Creates a ReminderPage for configuring a specific reminder for a person.
	/// Params: reminderIndex - The reminder index (0, 1, or 2), person - The person to edit reminders for
	/// </summary>
	public ReminderPage(int reminderIndex, Person? person)
	{
		InitializeComponent();
		_reminderIndex = reminderIndex;
		_person = person;
		_isWizardMode = person == null;

		SetupHeader();
		SetupDaysPicker();
		SetupTimeLabels();
		LoadReminder();
		UpdateButtonText();
		UpdateStatusLabels();
	}
	#endregion

	#region Private Methods
	private void SetupHeader()
	{
		int displayNumber = _reminderIndex + 1;
		ReminderHeaderLabel.Text = string.Format(
			MobileLanguages.Resources.Settings_ReminderNumber,
			displayNumber);
	}

	private void SetupTimeLabels()
	{
		string timezoneAbbr = DeviceService.GetTimeZoneAbbreviation();
		string timeLabel = string.Format(MobileLanguages.Resources.General_Label_Time, timezoneAbbr);
		NotificationTimeLabel.Text = timeLabel;
		AlarmTimeLabel.Text = timeLabel;
		EmailTimeLabel.Text = timeLabel;
		SignalTimeLabel.Text = timeLabel;
		SmsTimeLabel.Text = timeLabel;
		WhatsAppTimeLabel.Text = timeLabel;
	}

	private void SetupDaysPicker()
	{
		// Get days already used by other reminders
		var usedDays = new HashSet<int>();
		if (_person != null)
		{
			// Person mode: check person's other reminders
			for (int i = 0; i < _person.Reminders.Length; i++)
			{
				if (i != _reminderIndex && _person.Reminders[i] != null)
				{
					usedDays.Add(_person.Reminders[i]!.DaysBefore);
				}
			}
		}
		else
		{
			// Wizard mode: check account's reminder templates
			for (int i = 0; i < App.Account.ReminderTemplates.Length; i++)
			{
				if (i != _reminderIndex && App.Account.ReminderTemplates[i] != null)
				{
					usedDays.Add(App.Account.ReminderTemplates[i]!.DaysBefore);
				}
			}
		}

		// Get current reminder's days (to include in list)
		var currentReminder = GetCurrentReminder();
		_currentDays = currentReminder?.DaysBefore ?? GetDefaultDays();

		// Build available days list (0-30), excluding used days but including current
		_availableDays = [];
		for (int i = 0; i < MobileConstants.MAX_REMINDER_DAYS + 1; i++)
		{
			if (!usedDays.Contains(i) || i == _currentDays)
			{
				_availableDays.Add(i);
			}
		}

		// Sort descending (largest first)
		_availableDays = _availableDays.OrderByDescending(d => d).ToList();

		DaysPicker.ItemsSource = _availableDays;

		// Select current value
		int index = _availableDays.IndexOf(_currentDays);
		DaysPicker.SelectedIndex = index >= 0 ? index : 0;
	}

	private int GetDefaultDays()
	{
		var result = _reminderIndex switch
		{
			0 => 0,
			1 => 3,
			2 => 7,
			_ => 0
		};

		return result;
	}

	private void UpdateButtonText()
	{
		if (!_isWizardMode)
		{
			NextButton.Text = MobileLanguages.Resources.General_Button_Save;
		}
	}

	private void UpdateStatusLabels()
	{
		// Hide status labels when user has a valid subscription
		bool hasSubscription = App.Account.Subscription != SubscriptionTier.Free;
		NotificationStatusLabel.IsVisible = !hasSubscription;
		AlarmStatusLabel.IsVisible = !hasSubscription;
		EmailStatusLabel.IsVisible = !hasSubscription;
		SignalStatusLabel.IsVisible = !hasSubscription;
		SmsStatusLabel.IsVisible = !hasSubscription;
		WhatsAppStatusLabel.IsVisible = !hasSubscription;
	}

	private void LoadReminder()
	{
		var reminder = GetCurrentReminder();
		if (reminder != null)
		{
			// Local methods
			var notification = reminder.LocalMethods.FirstOrDefault(m => m.Type == LocalMethodType.Notification);
			var alarm = reminder.LocalMethods.FirstOrDefault(m => m.Type == LocalMethodType.Alarm);
			NotificationSwitch.IsToggled = notification?.Enabled ?? false;
			AlarmSwitch.IsToggled = alarm?.Enabled ?? false;
			NotificationTimePicker.Time = MinutesToTimeSpan(notification?.TimeMinutes ?? ReminderMethodConfig.Local[LocalMethodType.Notification].DefaultTime);
			AlarmTimePicker.Time = MinutesToTimeSpan(alarm?.TimeMinutes ?? ReminderMethodConfig.Local[LocalMethodType.Alarm].DefaultTime);

			// External methods
			var email = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Email);
			var sms = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Sms);
			var whatsApp = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.WhatsApp);
			var signal = reminder.ExternalMethods.FirstOrDefault(m => m.Type == ExternalMethodType.Signal);
			EmailSwitch.IsToggled = email?.Enabled ?? false;
			SmsSwitch.IsToggled = sms?.Enabled ?? false;
			WhatsAppSwitch.IsToggled = whatsApp?.Enabled ?? false;
			SignalSwitch.IsToggled = signal?.Enabled ?? false;
			EmailTimePicker.Time = MinutesToTimeSpan(email?.TimeMinutes ?? ReminderMethodConfig.External[ExternalMethodType.Email].DefaultTime);
			SmsTimePicker.Time = MinutesToTimeSpan(sms?.TimeMinutes ?? ReminderMethodConfig.External[ExternalMethodType.Sms].DefaultTime);
			WhatsAppTimePicker.Time = MinutesToTimeSpan(whatsApp?.TimeMinutes ?? ReminderMethodConfig.External[ExternalMethodType.WhatsApp].DefaultTime);
			SignalTimePicker.Time = MinutesToTimeSpan(signal?.TimeMinutes ?? ReminderMethodConfig.External[ExternalMethodType.Signal].DefaultTime);
		}
		else
		{
			// Set defaults for new reminder
			NotificationTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.Local[LocalMethodType.Notification].DefaultTime);
			AlarmTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.Local[LocalMethodType.Alarm].DefaultTime);
			EmailTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.External[ExternalMethodType.Email].DefaultTime);
			SmsTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.External[ExternalMethodType.Sms].DefaultTime);
			WhatsAppTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.External[ExternalMethodType.WhatsApp].DefaultTime);
			SignalTimePicker.Time = MinutesToTimeSpan(ReminderMethodConfig.External[ExternalMethodType.Signal].DefaultTime);
		}
	}

	private static TimeSpan MinutesToTimeSpan(int minutes)
	{
		int hours = minutes / 60;
		int mins = minutes % 60;
		var result = new TimeSpan(hours, mins, 0);
		return result;
	}

	private static int TimeSpanToMinutes(TimeSpan time)
	{
		int result = (int)time.TotalMinutes;
		return result;
	}

	private Reminder? GetCurrentReminder()
	{
		if (_person != null)
		{
			return _person.Reminders[_reminderIndex];
		}
		else
		{
			return App.Account.ReminderTemplates[_reminderIndex];
		}
	}

	private void SaveCurrentReminder(Reminder reminder)
	{
		if (_person != null)
		{
			_person.Reminders[_reminderIndex] = reminder;
			PersonService.Update(_person);
		}
		else
		{
			App.Account.ReminderTemplates[_reminderIndex] = reminder;
		}
	}

	private Reminder CreateReminder()
	{
		int days = DaysPicker.SelectedIndex >= 0 && DaysPicker.SelectedIndex < _availableDays.Count
			? _availableDays[DaysPicker.SelectedIndex]
			: 0;

		var reminder = new Reminder
		{
			DaysBefore = days,
			LocalMethods =
			[
				new ReminderMethodLocal
				{
					Type = LocalMethodType.Notification,
					Enabled = NotificationSwitch.IsToggled,
					TimeMinutes = TimeSpanToMinutes(NotificationTimePicker.Time),
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
					TimeMinutes = TimeSpanToMinutes(AlarmTimePicker.Time),
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
					TimeMinutes = TimeSpanToMinutes(EmailTimePicker.Time),
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.Sms,
					Enabled = SmsSwitch.IsToggled,
					TimeMinutes = TimeSpanToMinutes(SmsTimePicker.Time),
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.WhatsApp,
					Enabled = WhatsAppSwitch.IsToggled,
					TimeMinutes = TimeSpanToMinutes(WhatsAppTimePicker.Time),
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				},
				new ReminderMethodExternal
				{
					Type = ExternalMethodType.Signal,
					Enabled = SignalSwitch.IsToggled,
					TimeMinutes = TimeSpanToMinutes(SignalTimePicker.Time),
					IncludeAge = CommonConstants.DEFAULT_MESSAGE_INCLUDE_AGE
				}
			]
		};

		return reminder;
	}
	#endregion

	#region Event Handlers
	private async void OnNotificationInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderNotification.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnAlarmInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderAlarm.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnEmailInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderEmail.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnSignalInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderSignal.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnSmsInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderSms.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnWhatsAppInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.ReminderWhatsApp.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		if (!_isWizardMode)
		{
			// Person mode: go back
			await App.GoBackAsync();
			return;
		}

		// Wizard mode
		switch (_reminderIndex)
		{
			case 0:
				App.SetRootPage(new StartPage_5());
				break;
			case 1:
				App.SetRootPage(new ReminderPage(0));
				break;
			case 2:
				App.SetRootPage(new ReminderPage(1));
				break;
		}
	}

	private async void OnNextClicked(object? sender, EventArgs e)
	{
		var reminder = CreateReminder();
		SaveCurrentReminder(reminder);

		if (!_isWizardMode)
		{
			// Person mode: go back
			App.ForwardPageType = null;
			await App.GoBackAsync();
			return;
		}

		// Wizard mode
		int reminderCount = App.Account.RequestedReminderTemplatesCount;
		int nextIndex = _reminderIndex + 1;

		if (nextIndex < reminderCount)
		{
			App.SetRootPage(new ReminderPage(nextIndex));
		}
		else
		{
			App.SetRootPage(new StartPage_9());
		}
	}
	#endregion
}

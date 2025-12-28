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
		LoadReminder();
		UpdateButtonText();
	}
	#endregion

	#region Private Methods
	private void SetupHeader()
	{
		ReminderHeaderLabel.Text = _reminderIndex switch
		{
			0 => MobileLanguages.Resources.Settings_Reminder_1,
			1 => MobileLanguages.Resources.Settings_Reminder_2,
			2 => MobileLanguages.Resources.Settings_Reminder_3,
			_ => MobileLanguages.Resources.Settings_Reminder_1
		};
	}

	private void SetupDaysPicker()
	{
		// Get days already used by other reminders
		var usedDays = ReminderService.GetUsedDays(_person, _reminderIndex);

		// Get current reminder's days (to include in list)
		var currentReminder = GetCurrentReminder();
		_currentDays = currentReminder?.DaysBefore ?? GetDefaultDays();

		// Build available days list (0-30), excluding used days but including current
		_availableDays = [];
		for (int i = 0; i < MobileConstants.MAX_REMINDER_DAYS; i++)
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
			0 => 7,
			1 => 3,
			2 => 0,
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
	#endregion

	#region Event Handlers
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

namespace Mobile;

public partial class StartPage_4 : ContentPage
{
	#region Fields
	private readonly List<string> _hoursList = [];
	private readonly List<string> _minutesList = [];
	private bool _isLoading = true;
	#endregion

	#region Constructor
	public StartPage_4()
	{
		InitializeComponent();
		InitializePickers();
		LoadSettings();
		_isLoading = false;
	}
	#endregion

	#region Initialize
	private void InitializePickers()
	{
		for (int i = 0; i < 24; i++)
			_hoursList.Add(i.ToString("D2"));

		for (int i = 0; i < 60; i++)
			_minutesList.Add(i.ToString("D2"));

		EmailHoursPicker.ItemsSource = _hoursList;
		EmailMinutesPicker.ItemsSource = _minutesList;

		SmsHoursPicker.ItemsSource = _hoursList;
		SmsMinutesPicker.ItemsSource = _minutesList;

		LockScreenHoursPicker.ItemsSource = _hoursList;
		LockScreenMinutesPicker.ItemsSource = _minutesList;

		WhatsAppHoursPicker.ItemsSource = _hoursList;
		WhatsAppMinutesPicker.ItemsSource = _minutesList;
	}
	#endregion

	#region Load
	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		SetTimePicker(EmailHoursPicker, EmailMinutesPicker, settings.ReminderTimeEmail);
		SetTimePicker(SmsHoursPicker, SmsMinutesPicker, settings.ReminderTimeSms);
		SetTimePicker(LockScreenHoursPicker, LockScreenMinutesPicker, settings.ReminderTimeLockScreen);
		SetTimePicker(WhatsAppHoursPicker, WhatsAppMinutesPicker, settings.ReminderTimeWhatsApp);

		EmailSwitch.IsToggled = settings.ReminderEmailEnabled;
		SmsSwitch.IsToggled = settings.ReminderSmsEnabled;
		LockScreenSwitch.IsToggled = settings.ReminderLockScreenEnabled;
		WhatsAppSwitch.IsToggled = settings.ReminderWhatsAppEnabled;

		bool noReminders = !settings.ReminderEmailEnabled &&
		                   !settings.ReminderSmsEnabled &&
		                   !settings.ReminderLockScreenEnabled &&
		                   !settings.ReminderWhatsAppEnabled;
		NoRemindersSwitch.IsToggled = noReminders;
		UpdateReminderMethodsVisibility();
	}

	private static void SetTimePicker(Picker hoursPicker, Picker minutesPicker, int time)
	{
		int hours = time / 100;
		int minutes = time % 100;
		hoursPicker.SelectedIndex = hours;
		minutesPicker.SelectedIndex = minutes;
	}
	#endregion

	#region Events
	private void OnNoRemindersToggled(object? sender, ToggledEventArgs e)
	{
		if (_isLoading)
			return;

		if (e.Value)
		{
			EmailSwitch.IsToggled = false;
			SmsSwitch.IsToggled = false;
			LockScreenSwitch.IsToggled = false;
			WhatsAppSwitch.IsToggled = false;
		}

		UpdateReminderMethodsVisibility();
	}

	private void UpdateReminderMethodsVisibility()
	{
		ReminderMethodsContainer.IsVisible = !NoRemindersSwitch.IsToggled;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_3();
		}
	}

	private void OnFinishClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		settings.ReminderTimeEmail = GetTimeFromPickers(EmailHoursPicker, EmailMinutesPicker);
		settings.ReminderTimeSms = GetTimeFromPickers(SmsHoursPicker, SmsMinutesPicker);
		settings.ReminderTimeLockScreen = GetTimeFromPickers(LockScreenHoursPicker, LockScreenMinutesPicker);
		settings.ReminderTimeWhatsApp = GetTimeFromPickers(WhatsAppHoursPicker, WhatsAppMinutesPicker);

		settings.ReminderEmailEnabled = EmailSwitch.IsToggled;
		settings.ReminderSmsEnabled = SmsSwitch.IsToggled;
		settings.ReminderLockScreenEnabled = LockScreenSwitch.IsToggled;
		settings.ReminderWhatsAppEnabled = WhatsAppSwitch.IsToggled;

		SettingsService.Update(settings);

		PrefsHelper.SetValue(MobileConstants.PREFS_SETTINGS_INITIALIZED, true);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}

	private static int GetTimeFromPickers(Picker hoursPicker, Picker minutesPicker)
	{
		int hours = hoursPicker.SelectedIndex;
		int minutes = minutesPicker.SelectedIndex;
		int result = hours * 100 + minutes;
		return result;
	}
	#endregion
}

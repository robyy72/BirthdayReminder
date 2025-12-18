namespace Mobile;

public partial class StartPage_5 : ContentPage
{
	private bool _isLoading = true;

	public StartPage_5()
	{
		InitializeComponent();
		LoadSettings();
		_isLoading = false;
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		bool noReminders = !settings.ReminderEmailEnabled &&
		                   !settings.ReminderSmsEnabled &&
		                   !settings.ReminderLockScreenEnabled &&
		                   !settings.ReminderWhatsAppEnabled;
		NoReminderSwitch.IsToggled = noReminders;

		EmailSwitch.IsToggled = settings.ReminderEmailEnabled;
		SmsSwitch.IsToggled = settings.ReminderSmsEnabled;
		LockScreenSwitch.IsToggled = settings.ReminderLockScreenEnabled;
		WhatsAppSwitch.IsToggled = settings.ReminderWhatsAppEnabled;

		EmailTimePicker.Time = IntToTimeSpan(settings.ReminderTimeEmail);
		SmsTimePicker.Time = IntToTimeSpan(settings.ReminderTimeSms);
		LockScreenTimePicker.Time = IntToTimeSpan(settings.ReminderTimeLockScreen);
		WhatsAppTimePicker.Time = IntToTimeSpan(settings.ReminderTimeWhatsApp);

		UpdateReminderVisibility();
	}

	private static TimeSpan IntToTimeSpan(int time)
	{
		int hours = time / 100;
		int minutes = time % 100;
		var result = new TimeSpan(hours, minutes, 0);
		return result;
	}

	private static int TimeSpanToInt(TimeSpan time)
	{
		int result = time.Hours * 100 + time.Minutes;
		return result;
	}

	private void OnNoReminderToggled(object? sender, ToggledEventArgs e)
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

		UpdateReminderVisibility();
	}

	private void OnReminderToggled(object? sender, ToggledEventArgs e)
	{
		if (_isLoading)
			return;

		if (e.Value && NoReminderSwitch.IsToggled)
		{
			NoReminderSwitch.IsToggled = false;
		}

		UpdateReminderVisibility();
	}

	private void UpdateReminderVisibility()
	{
		ReminderMethodsContainer.IsVisible = !NoReminderSwitch.IsToggled;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_4();
		}
	}

	private void OnFinishClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		settings.ReminderEmailEnabled = EmailSwitch.IsToggled;
		settings.ReminderSmsEnabled = SmsSwitch.IsToggled;
		settings.ReminderLockScreenEnabled = LockScreenSwitch.IsToggled;
		settings.ReminderWhatsAppEnabled = WhatsAppSwitch.IsToggled;

		settings.ReminderTimeEmail = TimeSpanToInt(EmailTimePicker.Time);
		settings.ReminderTimeSms = TimeSpanToInt(SmsTimePicker.Time);
		settings.ReminderTimeLockScreen = TimeSpanToInt(LockScreenTimePicker.Time);
		settings.ReminderTimeWhatsApp = TimeSpanToInt(WhatsAppTimePicker.Time);

		SettingsService.Update(settings);

		PrefsHelper.SetValue(MobileConstants.PREFS_SETTINGS_INITIALIZED, true);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}
}

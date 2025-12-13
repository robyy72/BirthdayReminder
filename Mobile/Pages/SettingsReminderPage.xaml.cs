namespace Mobile;

public partial class SettingsReminderPage : ContentPage
{
	#region Fields
	private readonly List<string> _hoursList = [];
	private readonly List<string> _minutesList = [];
	#endregion

	#region Constructor
	public SettingsReminderPage()
	{
		InitializeComponent();
		InitializePickers();
		LoadSettings();
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

		RemindUntilApprovedSwitch.IsToggled = settings.RemindUntilApproved;
	}

	private static void SetTimePicker(Picker hoursPicker, Picker minutesPicker, int time)
	{
		int hours = time / 100;
		int minutes = time % 100;
		hoursPicker.SelectedIndex = hours;
		minutesPicker.SelectedIndex = minutes;
	}
	#endregion

	#region Save
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		settings.ReminderTimeEmail = GetTimeFromPickers(EmailHoursPicker, EmailMinutesPicker);
		settings.ReminderTimeSms = GetTimeFromPickers(SmsHoursPicker, SmsMinutesPicker);
		settings.ReminderTimeLockScreen = GetTimeFromPickers(LockScreenHoursPicker, LockScreenMinutesPicker);
		settings.ReminderTimeWhatsApp = GetTimeFromPickers(WhatsAppHoursPicker, WhatsAppMinutesPicker);

		settings.RemindUntilApproved = RemindUntilApprovedSwitch.IsToggled;

		SettingsService.Update(settings);

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
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

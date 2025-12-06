namespace Mobile;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		LoadSettings();
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		SourcePicker.SelectedIndex = settings.BirthdaySource switch
		{
			BirthdaySource.ManualInput => 0,
			BirthdaySource.Contacts => 1,
			_ => 0
		};

		var hours = settings.DefaultReminderTime / 100;
		var minutes = settings.DefaultReminderTime % 100;
		ReminderTimePicker.Time = new TimeSpan(hours, minutes, 0);

		LocalePicker.SelectedItem = settings.Locale;
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		settings.BirthdaySource = SourcePicker.SelectedIndex switch
		{
			0 => BirthdaySource.ManualInput,
			1 => BirthdaySource.Contacts,
			_ => BirthdaySource.ManualInput
		};

		settings.DefaultReminderTime = ReminderTimePicker.Time.Hours * 100 + ReminderTimePicker.Time.Minutes;

		if (LocalePicker.SelectedItem is string locale)
		{
			settings.Locale = locale;
		}

		SettingsService.Update(settings);

		await DisplayAlert("Settings", "Settings saved", "OK");
	}
}

namespace Mobile;

public partial class Welcome_1Page : ContentPage
{
	private int _hours = 18;
	private int _minutes = 0;

	public Welcome_1Page()
	{
		InitializeComponent();
		LoadSettings();
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		_hours = settings.DefaultReminderTime / 100;
		_minutes = settings.DefaultReminderTime % 100;

		HoursEntry.Text = _hours.ToString("D2");
		MinutesEntry.Text = _minutes.ToString("D2");

		if (settings.Locale == "en")
		{
			RadioEn.IsChecked = true;
		}
		else
		{
			RadioDe.IsChecked = true;
		}
	}

	private void OnHoursUp(object? sender, EventArgs e)
	{
		_hours = (_hours + 1) % 24;
		HoursEntry.Text = _hours.ToString("D2");
	}

	private void OnHoursDown(object? sender, EventArgs e)
	{
		_hours = (_hours - 1 + 24) % 24;
		HoursEntry.Text = _hours.ToString("D2");
	}

	private void OnMinutesUp(object? sender, EventArgs e)
	{
		_minutes = (_minutes + 5) % 60;
		MinutesEntry.Text = _minutes.ToString("D2");
	}

	private void OnMinutesDown(object? sender, EventArgs e)
	{
		_minutes = (_minutes - 5 + 60) % 60;
		MinutesEntry.Text = _minutes.ToString("D2");
	}

	private async void OnNextClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		if (int.TryParse(HoursEntry.Text, out int hours))
		{
			_hours = Math.Clamp(hours, 0, 23);
		}
		if (int.TryParse(MinutesEntry.Text, out int minutes))
		{
			_minutes = Math.Clamp(minutes, 0, 59);
		}

		settings.DefaultReminderTime = _hours * 100 + _minutes;

		if (RadioEn.IsChecked)
		{
			settings.Locale = "en";
		}
		else
		{
			settings.Locale = "de";
		}

		SettingsService.Update(settings);

		await Shell.Current.GoToAsync("//Welcome_2Page");
	}
}

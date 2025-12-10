namespace Mobile;

public partial class Welcome_1Page : ContentPage
{
	private readonly List<string> _hoursList = [];
	private readonly List<string> _minutesList = [];
	private bool _pickersInitialized = false;

	public Welcome_1Page()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (!_pickersInitialized)
		{
			InitializePickers();
			_pickersInitialized = true;
		}
		LoadSettings();
	}

	private void InitializePickers()
	{
		for (int i = 0; i < 24; i++)
			_hoursList.Add(i.ToString("D2"));

		for (int i = 0; i < 60; i++)
			_minutesList.Add(i.ToString("D2"));

		HoursPicker.ItemsSource = _hoursList;
		MinutesPicker.ItemsSource = _minutesList;
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		int hours = settings.DefaultReminderTime / 100;
		int minutes = settings.DefaultReminderTime % 100;

		HoursPicker.SelectedIndex = hours;
		MinutesPicker.SelectedIndex = minutes;

		if (settings.Locale == "en")
			RadioEn.IsChecked = true;
		else
			RadioDe.IsChecked = true;

		switch (settings.Theme)
		{
			case "Light":
				RadioLight.IsChecked = true;
				break;
			case "Dark":
				RadioDark.IsChecked = true;
				break;
			default:
				if (DeviceService.IsDarkTheme())
					RadioDark.IsChecked = true;
				else
					RadioLight.IsChecked = true;
				break;
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		int hours = HoursPicker.SelectedIndex;
		int minutes = MinutesPicker.SelectedIndex;
		settings.DefaultReminderTime = hours * 100 + minutes;

		if (RadioEn.IsChecked)
			settings.Locale = "en";
		else
			settings.Locale = "de";

		if (RadioLight.IsChecked)
			settings.Theme = "Light";
		else if (RadioDark.IsChecked)
			settings.Theme = "Dark";
		else
			settings.Theme = "System";

		DeviceService.ApplyTheme(settings.Theme);
		SettingsService.Update(settings);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new Welcome_2Page();
		}
	}
}

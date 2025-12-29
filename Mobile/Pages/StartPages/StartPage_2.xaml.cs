namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	#region Fields
	private List<TimeZoneInfo> _timezones = [];
	private const string DEVICE_TIMEZONE_PREFIX = "[Device] ";
	#endregion

	#region Constructor
	public StartPage_2()
	{
		InitializeComponent();
		LoadTimezones();
		LoadAccount();
	}
	#endregion

	#region Private Methods
	private void LoadTimezones()
	{
		_timezones = TimeZoneInfo.GetSystemTimeZones().ToList();

		// Add device timezone at top with prefix
		var deviceTz = TimeZoneInfo.Local;
		var items = new List<string>
		{
			DEVICE_TIMEZONE_PREFIX + deviceTz.DisplayName
		};

		// Add all other timezones
		items.AddRange(_timezones.Select(tz => tz.DisplayName));

		TimezonePicker.ItemsSource = items;
	}

	private void LoadAccount()
	{
		// Theme
		switch (App.Account.Theme)
		{
			case "Light":
				RadioThemeLight.IsChecked = true;
				break;
			case "Dark":
				RadioThemeDark.IsChecked = true;
				break;
			default:
				RadioThemeSystem.IsChecked = true;
				break;
		}

		// Timezone
		if (string.IsNullOrEmpty(App.Account.TimeZoneId))
		{
			// Default to device timezone
			TimezonePicker.SelectedIndex = 0;
		}
		else
		{
			// Find matching timezone
			var index = _timezones.FindIndex(tz => tz.Id == App.Account.TimeZoneId);
			if (index >= 0)
			{
				TimezonePicker.SelectedIndex = index + 1; // +1 because of device timezone at top
			}
			else
			{
				TimezonePicker.SelectedIndex = 0;
			}
		}
	}

	private void SaveSettings()
	{
		// Save theme
		if (RadioThemeLight.IsChecked)
			App.Account.Theme = "Light";
		else if (RadioThemeDark.IsChecked)
			App.Account.Theme = "Dark";
		else
			App.Account.Theme = "System";

		// Save timezone
		if (TimezonePicker.SelectedIndex == 0)
		{
			// Device timezone
			App.Account.TimeZoneId = TimeZoneInfo.Local.Id;
		}
		else if (TimezonePicker.SelectedIndex > 0)
		{
			App.Account.TimeZoneId = _timezones[TimezonePicker.SelectedIndex - 1].Id;
		}

		AccountService.Save();

		// Apply theme immediately
		DeviceService.ApplyTheme(App.Account.Theme);
	}
	#endregion

	#region Event Handlers
	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_1());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		SaveSettings();
		App.SetRootPage(new StartPage_3());
	}

	private async void OnThemeInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.Theme.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnTimezoneInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.Timezone.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}
	#endregion
}

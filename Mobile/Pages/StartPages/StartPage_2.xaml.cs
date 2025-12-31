namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	#region Constructor
	public StartPage_2()
	{
		InitializeComponent();
		LoadSettings();
	}
	#endregion

	#region Private Methods
	private void LoadSettings()
	{
		// Language
		switch (App.Account.Locale)
		{
			case "de":
				RadioLanguageDe.IsChecked = true;
				break;
			default:
				RadioLanguageEn.IsChecked = true;
				break;
		}

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
		TimezonePicker.ItemsSource = TimezoneService.GetDisplayNames();
		TimezonePicker.SelectedIndex = TimezoneService.GetIndex(App.Account.TimeZoneId);
	}

	private void SaveSettings()
	{
		// Save language
		switch (RadioLanguageEn.IsChecked)
		{
			case true:
				App.Account.Locale = "en";
				break;
			default:
				App.Account.Locale = "de";
				break;
		}

		// Save theme
		switch (true)
		{
			case true when RadioThemeLight.IsChecked:
				App.Account.Theme = "Light";
				break;
			case true when RadioThemeDark.IsChecked:
				App.Account.Theme = "Dark";
				break;
			default:
				App.Account.Theme = "System";
				break;
		}

		// Save timezone
		App.Account.TimeZoneId = TimezoneService.GetIdByIndex(TimezonePicker.SelectedIndex);

		AccountService.Save();
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

	private async void OnTimezoneInfoTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.Timezone.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}
	#endregion
}

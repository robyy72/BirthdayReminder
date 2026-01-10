namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	#region Private Fields
	private bool _isLoading = true;
	#endregion

	#region Constructor
	public StartPage_2()
	{
		InitializeComponent();
		LoadSettings();
		_isLoading = false;
	}
	#endregion

	#region Private Methods
	private void LoadSettings()
	{
		// Language - detect from device if not explicitly set
		string locale = GetEffectiveLocale();
		switch (locale)
		{
			case "de":
				RadioLanguageDe.IsChecked = true;
				break;
			default:
				RadioLanguageEn.IsChecked = true;
				break;
		}

		// Theme - use system theme if "System" or not explicitly set
		string theme = GetEffectiveTheme();
		switch (theme)
		{
			case "Dark":
				RadioThemeDark.IsChecked = true;
				break;
			default:
				RadioThemeLight.IsChecked = true;
				break;
		}

		// Timezone
		TimezonePicker.ItemsSource = TimezoneService.GetDisplayNames();
		TimezonePicker.SelectedIndex = TimezoneService.GetIndex(App.Account.TimeZoneId);
	}

	private string GetEffectiveLocale()
	{
		// If explicitly set to a supported locale, use it
		if (App.Account.Locale == "de" || App.Account.Locale == "en")
		{
			// Check if this is a first run (locale still at default)
			bool isFirstRun = !Preferences.ContainsKey(MobileConstants.PREFS_ACCOUNT_INITIALIZED);
			if (!isFirstRun)
			{
				return App.Account.Locale;
			}
		}

		// Detect from device
		string deviceLocale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		string effectiveLocale = deviceLocale == "de" ? "de" : "en";
		return effectiveLocale;
	}

	private string GetEffectiveTheme()
	{
		// If explicitly set to Light or Dark, use it
		if (App.Account.Theme == "Light" || App.Account.Theme == "Dark")
		{
			return App.Account.Theme;
		}

		// Detect from system
		bool isDark = DeviceService.IsDarkTheme();
		string effectiveTheme = isDark ? "Dark" : "Light";
		return effectiveTheme;
	}

	private void SaveSettings()
	{
		// Save language
		App.Account.Locale = RadioLanguageEn.IsChecked == true ? "en" : "de";

		// Save theme
		App.Account.Theme = RadioThemeDark.IsChecked == true ? "Dark" : "Light";

		// Save timezone
		App.Account.TimeZoneId = TimezoneService.GetIdByIndex(TimezonePicker.SelectedIndex);

		AccountService.Save();
		DeviceService.ApplyTheme(App.Account.Theme);
	}
	#endregion

	#region Event Handlers
	private void OnLanguageChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value) return;

		string locale = RadioLanguageDe.IsChecked == true ? "de" : "en";
		App.Account.Locale = locale;

		var culture = new System.Globalization.CultureInfo(locale);
		System.Globalization.CultureInfo.CurrentCulture = culture;
		System.Globalization.CultureInfo.CurrentUICulture = culture;
		MobileLanguages.Resources.Culture = culture;

		// Reload page to apply new language
		App.SetRootPage(new StartPage_2());
	}

	private void OnThemeChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value) return;

		string theme = RadioThemeDark.IsChecked == true ? "Dark" : "Light";
		App.Account.Theme = theme;
		DeviceService.ApplyTheme(theme);
	}

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
		var browserPage = new BrowserPage(
			MobileConstants.URL_TIMEZONE_INFO,
			MobileLanguages.Resources.Timezone_MoreInfo);
		await Navigation.PushModalAsync(browserPage);
	}
	#endregion
}





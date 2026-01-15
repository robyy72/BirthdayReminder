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
		// Language
		UpdateLanguageLabel();

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
		UpdateTimezoneLabel();
	}

	private void UpdateLanguageLabel()
	{
		string locale = GetEffectiveLocale();
		CurrentLanguageLabel.Text = locale == "de" ? "Deutsch" : "English";
	}

	private void UpdateTimezoneLabel()
	{
		var displayNames = TimezoneService.GetDisplayNames();
		int index = TimezoneService.GetIndex(App.Account.TimeZoneId);
		if (index >= 0 && index < displayNames.Count)
		{
			CurrentTimezoneLabel.Text = displayNames[index];
		}
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
		// Save theme
		App.Account.Theme = RadioThemeDark.IsChecked == true ? "Dark" : "Light";

		// Language and Timezone are saved on their respective selection pages

		AccountService.Save();
		DeviceService.ApplyTheme(App.Account.Theme);
	}
	#endregion

	#region Event Handlers
	private void OnLanguageAdjustClicked(object? sender, EventArgs e)
	{
		App.BackwardPageType = typeof(StartPage_2);
		App.SetRootPage(new SelectLanguagePage());
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

	private void OnTimezoneAdjustClicked(object? sender, EventArgs e)
	{
		App.BackwardPageType = typeof(StartPage_2);
		App.SetRootPage(new SelectTimezonePage());
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateLanguageLabel();
		UpdateTimezoneLabel();
	}
	#endregion
}





namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	public StartPage_2()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadSettings();
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

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

	private void OnThemeChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (!e.Value)
			return;

		string theme = RadioLight.IsChecked ? "Light" : "Dark";
		DeviceService.ApplyTheme(theme);
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_1();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

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
			Application.Current.Windows[0].Page = new StartPage_3();
		}
	}
}

namespace Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(SetMainPage());
	}

	private static Page SetMainPage()
	{
		if (MobileConstants.CLEAN_PREFS_AT_START)
		{
			PrefsHelper.RemoveAllKeys();
		}

		if (!SettingsService.IsInitialized())
		{
			return new Welcome_1Page();
		}

		return new AppShell();
	}
}

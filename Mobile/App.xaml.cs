namespace Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Page page = SetMainPage();
        return new Window(page);
	}

	private static Page SetMainPage()
	{
		if (MobileConstants.CLEAN_PREFS_AT_START)		
			PrefsHelper.RemoveAllKeys();		

		Page page = new Welcome_1Page();
		if (SettingsService.IsInitialized())
			page = new AppShell();

		return page;
	}
}

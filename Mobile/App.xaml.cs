namespace Mobile;

public partial class App : Application
{
	public static bool NeedsReloadBirthdays { get; set; }

	public App()
	{
		InitializeComponent();
		ApplyTheme();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Page page = new StartPage_1();

		if (SettingsService.IsInitialized())
			page = new AppShell();

		Window window = new Window(page);
		return window;
	}

	private void ApplyTheme()
	{
		var settings = SettingsService.Get();
		DeviceService.ApplyTheme(settings.Theme);
	}
}

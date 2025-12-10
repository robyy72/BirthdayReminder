namespace Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Page page = new Welcome_1Page();
		
		if (SettingsService.IsInitialized())
			page = new AppShell();

		Window window = new Window(page);

        return window;
	}
}

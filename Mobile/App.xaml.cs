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
		if (!SettingsService.IsInitialized())
		{
			return new AskForContactsPage();
		}

		return new AppShell();
	}
}

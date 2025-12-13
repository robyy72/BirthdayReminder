namespace Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewBirthdayPage), typeof(NewBirthdayPage));
		Routing.RegisterRoute(nameof(SettingsReminderPage), typeof(SettingsReminderPage));
	}
}

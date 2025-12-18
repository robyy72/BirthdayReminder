namespace Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateEditBirthdayPage), typeof(CreateEditBirthdayPage));
		Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
	}
}

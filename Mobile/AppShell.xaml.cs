namespace Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewBirthdayPage), typeof(NewBirthdayPage));
	}
}

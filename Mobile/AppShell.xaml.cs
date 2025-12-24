namespace Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateEditBirthdayPage_1), typeof(CreateEditBirthdayPage_1));
		Routing.RegisterRoute(nameof(CreateEditBirthdayPage_2), typeof(CreateEditBirthdayPage_2));
		Routing.RegisterRoute(nameof(DeleteBirthdayPage), typeof(DeleteBirthdayPage));
		Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));

		VersionLabel.Text = $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
	}
}

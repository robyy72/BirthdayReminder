namespace Mobile;

public partial class AskForContactsPage : ContentPage
{
	public AskForContactsPage()
	{
		InitializeComponent();
	}

	private void OnManualInputClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();
		settings.BirthdaySource = BirthdaySource.ManualInput;
		SettingsService.Update(settings);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}

	private async void OnFromContactsClicked(object? sender, EventArgs e)
	{
		var status = await Permissions.RequestAsync<Permissions.ContactsRead>();

		var settings = SettingsService.Get();
		if (status == PermissionStatus.Granted)
		{
			settings.BirthdaySource = BirthdaySource.Contacts;
		}
		else
		{
			settings.BirthdaySource = BirthdaySource.ManualInput;
		}
		SettingsService.Update(settings);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}
}

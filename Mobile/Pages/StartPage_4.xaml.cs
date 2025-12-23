namespace Mobile;

public partial class StartPage_4 : ContentPage
{
	public StartPage_4()
	{
		InitializeComponent();
	}

	private async void OnRequestPermissionClicked(object? sender, EventArgs e)
	{
		bool granted = await DeviceService.RequestContactsReadPermissionAsync();

		PanelInitial.IsVisible = false;

		if (granted)
		{
			PanelGranted.IsVisible = true;
			// Keep user's ContactsReadMode selection from StartPage_3
		}
		else
		{
			PanelDenied.IsVisible = true;
			App.Account.ContactsReadMode = ContactsReadMode.None;
			AccountService.Save();
		}
		ButtonNext.IsEnabled = true;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_3();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_5();
		}
	}
}

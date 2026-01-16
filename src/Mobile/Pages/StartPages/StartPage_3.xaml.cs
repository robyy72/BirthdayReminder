namespace Mobile;

public partial class StartPage_3 : ContentPage
{
	#region Private Fields
	private bool _isLoading = true;
	private bool _permissionRequested = false;
	#endregion

	#region Constructor
	public StartPage_3()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}
	#endregion

	#region Private Methods
	private void LoadAccount()
	{
		// Pre-select "Use Contacts" if already allowed
		if (App.Account.ContactsReadMode > ContactsReadMode.None)
		{
			RadioAskNextPage.IsChecked = true;
		}
	}

	private void UpdatePermissionBoxVisibility()
	{
		PermissionBox.IsVisible = RadioAskNextPage.IsChecked;
	}
	#endregion

	#region Event Handlers
	private void OnRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value) return;
		UpdatePermissionBoxVisibility();
	}

	private async void OnPermissionButtonClicked(object? sender, EventArgs e)
	{
		if (_permissionRequested) return;
		_permissionRequested = true;

		PermissionButton.IsEnabled = false;
		PermissionButton.Text = MobileLanguages.Resources.Permission_Waiting;

		bool granted = await DeviceService.RequestContactsReadPermissionAsync();

		PermissionButton.IsVisible = false;
		PermissionResultLabel.IsVisible = true;

		if (granted)
		{
			PermissionResultLabel.Text = MobileLanguages.Resources.Permission_Granted;
			PermissionResultLabel.TextColor = Colors.Green;
			App.Account.ContactsReadMode = ContactsReadMode.ReadNamesWithBirthday;
			AccountService.Save();
		}
		else
		{
			PermissionResultLabel.Text = MobileLanguages.Resources.Permission_Denied;
			PermissionResultLabel.TextColor = Colors.Gray;
		}
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_2());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (RadioAskNextPage.IsChecked)
			App.SetRootPage(new StartPage_4());
		else
			App.SetRootPage(new StartPage_5());
	}
	#endregion
}

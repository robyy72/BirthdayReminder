namespace Mobile;

public partial class StartPage_3 : ContentPage
{
	#region Fields
	private bool _isLoading = true;
	#endregion

	#region Constructor
	public StartPage_3()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}
	#endregion

	private void LoadAccount()
	{
		// ContactsReadMode (default: ReadAllNames)
		if (App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday)
			RadioReadWithBirthday.IsChecked = true;
		else
			RadioReadAll.IsChecked = true;

		// ContactsReadWriteMode
		switch (App.Account.ContactsReadWriteMode)
		{
			case ContactsReadWriteMode.ReadAlways:
				RadioReadAlways.IsChecked = true;
				break;
			case ContactsReadWriteMode.ReadAlwaysAndAskWriteBack:
				RadioReadAlwaysAndAskWriteBack.IsChecked = true;
				break;
			case ContactsReadWriteMode.ReadAlwaysAndWriteBack:
				RadioReadAlwaysAndWriteBack.IsChecked = true;
				break;
			default:
				RadioReadOnlyOnce.IsChecked = true;
				break;
		}

		UpdateWriteModeOptions();
	}

	private void OnReadModeChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value)
			return;

		UpdateWriteModeOptions();
	}

	private void UpdateWriteModeOptions()
	{
		bool enableWriteOptions = RadioReadAll.IsChecked;

		// Enable/disable RadioButtons directly
		RadioReadAlwaysAndAskWriteBack.IsEnabled = enableWriteOptions;
		RadioReadAlwaysAndWriteBack.IsEnabled = enableWriteOptions;

		// Visual feedback
		GridOption3.Opacity = enableWriteOptions ? 1.0 : 0.4;
		GridOption4.Opacity = enableWriteOptions ? 1.0 : 0.4;

		// If write options disabled and option 3 or 4 selected, switch to option 2
		if (!enableWriteOptions &&
			(RadioReadAlwaysAndAskWriteBack.IsChecked || RadioReadAlwaysAndWriteBack.IsChecked))
		{
			RadioReadAlways.IsChecked = true;
		}
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_2();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		// Save ContactsReadMode selection
		if (RadioReadAll.IsChecked)
			App.Account.ContactsReadMode = ContactsReadMode.ReadAllNames;
		else
			App.Account.ContactsReadMode = ContactsReadMode.ReadNamesWithBirthday;

		// Save ContactsReadWriteMode selection
		if (RadioReadAlways.IsChecked)
			App.Account.ContactsReadWriteMode = ContactsReadWriteMode.ReadAlways;
		else if (RadioReadAlwaysAndAskWriteBack.IsChecked)
			App.Account.ContactsReadWriteMode = ContactsReadWriteMode.ReadAlwaysAndAskWriteBack;
		else if (RadioReadAlwaysAndWriteBack.IsChecked)
			App.Account.ContactsReadWriteMode = ContactsReadWriteMode.ReadAlwaysAndWriteBack;
		else
			App.Account.ContactsReadWriteMode = ContactsReadWriteMode.ReadOnlyOnce;

		AccountService.Save();

		// Navigate to permission request page
		if (Application.Current?.Windows.Count > 0)
		{
			App.BackwardPage = new StartPage_3();
			App.ForwardPage = new StartPage_5();
			Application.Current.Windows[0].Page = new RequestPermissionPage_1(PermissionType.Contacts);
		}
	}
}

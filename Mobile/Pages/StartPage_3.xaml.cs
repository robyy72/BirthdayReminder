namespace Mobile;

public partial class StartPage_3 : ContentPage
{
	public StartPage_3()
	{
		InitializeComponent();
		LoadAccount();
	}

	private void LoadAccount()
	{
		// ContactsReadMode
		if (App.Account.ContactsReadMode == ContactsReadMode.ReadAllNames)
			RadioReadAll.IsChecked = true;
		else
			RadioReadWithBirthday.IsChecked = true;

		// ContactsReadWriteMode (on Person, default to ReadOnlyOnce)
		// For now just default to ReadOnlyOnce
		RadioReadOnce.IsChecked = true;
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

		AccountService.Save();

		// Navigate to permission request page
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_4();
		}
	}
}

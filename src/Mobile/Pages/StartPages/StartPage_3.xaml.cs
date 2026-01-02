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
		// Pre-select "Use Contacts" if already allowed
		if (App.Account.ContactsReadMode > ContactsReadMode.None)
		{
			RadioAskNextPage.IsChecked = true;
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
}

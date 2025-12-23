namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	public StartPage_2()
	{
		InitializeComponent();
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_1();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.UseContacts = RadioAskNextPage.IsChecked;

		if (Application.Current?.Windows.Count > 0)
		{
			if (App.UseContacts)
				Application.Current.Windows[0].Page = new StartPage_3();
			else
				Application.Current.Windows[0].Page = new StartPage_5();
		}
	}
}

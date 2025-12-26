namespace Mobile;

public partial class StartPage_2 : ContentPage
{
	public StartPage_2()
	{
		InitializeComponent();
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_1());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (RadioAskNextPage.IsChecked)
			App.SetRootPage(new StartPage_3());
		else
			App.SetRootPage(new StartPage_5());
	}
}

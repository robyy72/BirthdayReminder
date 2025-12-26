namespace Mobile;

public partial class StartPage_1 : ContentPage
{
	public StartPage_1()
	{
		InitializeComponent();
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_2());
	}
}

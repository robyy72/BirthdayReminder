namespace Mobile;

public partial class StartPage_1 : ContentPage
{
	public StartPage_1()
	{
		InitializeComponent();
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_2();
		}
	}
}

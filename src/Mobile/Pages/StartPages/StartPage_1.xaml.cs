namespace Mobile;

public partial class StartPage_1 : ContentPage
{
	#region Constructor
	public StartPage_1()
	{
		InitializeComponent();
	}
	#endregion

	#region Events
	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_2());
	}
	#endregion
}

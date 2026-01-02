namespace Mobile;

public partial class RequestPermissionPage_2 : ContentPage
{
	#region Constructor
	public RequestPermissionPage_2()
	{
		InitializeComponent();
	}
	#endregion

	#region Event Handlers
	private void OnOpenSettingsClicked(object? sender, EventArgs e)
	{
		AppInfo.ShowSettingsUI();
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await App.PopPageAsync();
	}
	#endregion
}

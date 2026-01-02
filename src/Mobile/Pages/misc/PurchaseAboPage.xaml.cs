namespace Mobile;

public partial class PurchaseAboPage : ContentPage
{
	#region Constructor
	public PurchaseAboPage()
	{
		InitializeComponent();
	}
	#endregion

	#region Event Handlers
	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await App.GoBackAsync();
	}

	private async void OnPurchaseClicked(object? sender, EventArgs e)
	{
		// TODO: Implement in-app purchase flow
		await DisplayAlert(
			MobileLanguages.Resources.PurchaseAbo_Title,
			MobileLanguages.Resources.PurchaseAbo_ComingSoon,
			MobileLanguages.Resources.General_Button_OK);
	}
	#endregion
}

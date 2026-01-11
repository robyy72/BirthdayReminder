#region Usings
using System.Windows.Input;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Page shown when there is no internet connection.
/// </summary>
public partial class NoInternetPage : ContentPage
{
	#region Constructor
	public NoInternetPage()
	{
		InitializeComponent();
		Header.BackClickedCommand = new Command(OnBackClicked);
	}
	#endregion

	#region Event Handlers
	private async void OnBackClicked()
	{
		await NavigateToMainAsync();
	}

	private async void OnTryAgainClicked(object? sender, EventArgs e)
	{
		if (MobileService.HasNetworkAccess())
		{
			await App.GoBackAsync();
		}
		else
		{
			await DisplayAlert(
				MobileLanguages.Resources.NoInternet_Title,
				MobileLanguages.Resources.NoInternet_StillOffline,
				MobileLanguages.Resources.General_Button_OK);
		}
	}

	private async void OnContinueOfflineClicked(object? sender, EventArgs e)
	{
		await NavigateToMainAsync();
	}

	private async Task NavigateToMainAsync()
	{
		await App.NavigateToRootAsync();
	}
	#endregion
}

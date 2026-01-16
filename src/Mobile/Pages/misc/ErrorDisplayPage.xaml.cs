namespace Mobile;

/// <summary>
/// Aim: Page for displaying error information to the user.
/// </summary>
public partial class ErrorDisplayPage : ContentPage
{
	#region Fields
	private readonly string? _errorMessage;
	private readonly string? _errorDetails;
	private bool _detailsVisible = false;
	#endregion

	#region Constructor
	public ErrorDisplayPage(string? errorMessage = null, string? errorDetails = null)
	{
		InitializeComponent();
		_errorMessage = errorMessage;
		_errorDetails = errorDetails;
		Init();
	}
	#endregion

	#region Private Methods
	private void Init()
	{
		ErrorMessageLabel.Text = _errorMessage ?? MobileLanguages.Resources.Error_GenericMessage;

		if (!string.IsNullOrEmpty(_errorDetails))
		{
			DetailsLabel.Text = _errorDetails;
		}
		else
		{
			ShowDetailsLabel.IsVisible = false;
		}
	}
	#endregion

	#region Event Handlers
	private async void OnTryAgainClicked(object? sender, EventArgs e)
	{
		// Reload/retry the previous action
		await App.GoBackAsync();
	}

	private void OnExitAppClicked(object? sender, EventArgs e)
	{
		// Exit the application
		Application.Current?.Quit();
	}

	private async void OnBackToAppClicked(object? sender, EventArgs e)
	{
		// Navigate back to the app (main page)
		await App.GoBackAsync();
	}

	private void OnShowDetailsTapped(object? sender, EventArgs e)
	{
		_detailsVisible = !_detailsVisible;
		DetailsCard.IsVisible = _detailsVisible;
		ShowDetailsLabel.Text = _detailsVisible
			? MobileLanguages.Resources.Error_HideDetails
			: MobileLanguages.Resources.Error_ShowDetails;
	}
	#endregion
}

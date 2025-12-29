#region Usings
#endregion

namespace Mobile;

public partial class BrowserPage : ContentPage
{
	#region Private Fields
	private string _url = string.Empty;
	#endregion

	#region Properties
	public string Url
	{
		get => _url;
		set
		{
			_url = value;
			LoadUrl();
		}
	}

	public string HeaderTitle
	{
		get => HeaderLabel.Text;
		set => HeaderLabel.Text = value;
	}
	#endregion

	#region Constructor
	public BrowserPage()
	{
		InitializeComponent();
		BrowserWebView.Navigated += OnWebViewNavigated;
	}

	public BrowserPage(string url, string? headerTitle = null) : this()
	{
		_url = url;
		if (!string.IsNullOrEmpty(headerTitle))
		{
			HeaderLabel.Text = headerTitle;
		}
		LoadUrl();
	}
	#endregion

	#region Private Methods
	private void LoadUrl()
	{
		if (!string.IsNullOrEmpty(_url))
		{
			LoadingIndicator.IsVisible = true;
			LoadingIndicator.IsRunning = true;
			BrowserWebView.Source = _url;
		}
	}
	#endregion

	#region Event Handlers
	private void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
	{
		LoadingIndicator.IsVisible = false;
		LoadingIndicator.IsRunning = false;
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		if (Navigation.ModalStack.Count > 0)
		{
			await Navigation.PopModalAsync();
		}
		else
		{
			await App.GoBackAsync();
		}
	}

	private async void OnOpenExternalClicked(object? sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(_url))
		{
			try
			{
				await Browser.Default.OpenAsync(_url, BrowserLaunchMode.SystemPreferred);
			}
			catch
			{
				// Ignore if browser fails to open
			}
		}
	}
	#endregion
}

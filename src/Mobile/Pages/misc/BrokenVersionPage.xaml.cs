namespace Mobile;

/// <summary>
/// Aim: Page shown when the app version crashed before on this device.
/// </summary>
public partial class BrokenVersionPage : ContentPage
{
	public BrokenVersionPage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Aim: Open app store to check for updates.
	/// </summary>
	private async void OnCheckUpdateClicked(object? sender, EventArgs e)
	{
#if ANDROID
		await Launcher.OpenAsync($"market://details?id={AppInfo.PackageName}");
#elif IOS
		await Launcher.OpenAsync("https://apps.apple.com/app/id0000000000");
#endif
	}

	/// <summary>
	/// Aim: Clear broken marker and try to start the app anyway.
	/// </summary>
	private void OnTryAnywayClicked(object? sender, EventArgs e)
	{
		ErrorService.ClearBrokenVersionMarker();
		Application.Current!.Windows[0].Page = App.CreateMainNavigationPage();
	}
}

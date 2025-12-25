#region Usings
using Android.App;
using Android.Content;
using Android.Content.PM;
#endregion

namespace Mobile;

[Activity(
	Theme = "@style/Maui.SplashTheme",
	MainLauncher = true,
	LaunchMode = LaunchMode.SingleTop,
	ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	DataScheme = "birthdayreminder")]
public class MainActivity : MauiAppCompatActivity
{
	/// <summary>
	/// Aim: Verarbeitet Deep Links wenn die App bereits läuft
	/// </summary>
	protected override void OnNewIntent(Intent? intent)
	{
		base.OnNewIntent(intent);

		if (intent?.Data != null)
		{
			string? url = intent.Data.ToString();
			_ = DeepLinkRouter.HandleUrl(url);
		}
	}

	/// <summary>
	/// Aim: Verarbeitet Deep Links beim ersten Start der App
	/// </summary>
	protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		if (Intent?.Data != null)
		{
			string? url = Intent.Data.ToString();
			_ = DeepLinkRouter.HandleUrl(url);
		}
	}
}

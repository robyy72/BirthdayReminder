#region Usings
using Foundation;
using UIKit;
#endregion

namespace Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	/// <summary>
	/// Aim: Verarbeitet Deep Links wenn die App über URL-Schema geöffnet wird
	/// </summary>
	public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
	{
		if (url != null)
		{
			_ = DeepLinkService.HandleUrl(url.ToString());
		}

		return base.OpenUrl(application, url, options);
	}
}

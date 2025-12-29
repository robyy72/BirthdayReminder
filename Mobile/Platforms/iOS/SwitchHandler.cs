#region Usings
using Microsoft.Maui.Handlers;
using UIKit;
#endregion

namespace Mobile;

public static class SwitchHandlerConfiguration
{
	public static void ConfigureSwitchHandler()
	{
		SwitchHandler.Mapper.AppendToMapping("CustomSwitch", (handler, view) =>
		{
			var uiSwitch = handler.PlatformView;

			// Gray300 (#ACACAC) for off-track color - matches "unused" style
			uiSwitch.BackgroundColor = UIColor.FromRGB(0xAC, 0xAC, 0xAC);
			uiSwitch.Layer.CornerRadius = 16f; // Standard iOS switch corner radius
		});
	}
}

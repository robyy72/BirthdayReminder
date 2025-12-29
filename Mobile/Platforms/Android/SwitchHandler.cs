#region Usings
using Android.Content.Res;
using Android.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
#endregion

namespace Mobile;

public static class SwitchHandlerConfiguration
{
	public static void ConfigureSwitchHandler()
	{
		SwitchHandler.Mapper.AppendToMapping("CustomSwitch", (handler, view) =>
		{
			var switchView = handler.PlatformView;

			// Gray300 (#ACACAC) for off-track color - matches "unused" style
			var offTrackColor = Android.Graphics.Color.ParseColor("#ACACAC");

			// Create color state list for track: off = gray, on = controlled by OnColor
			var trackStates = new int[][]
			{
				new int[] { -Android.Resource.Attribute.StateChecked }, // unchecked (off)
				new int[] { Android.Resource.Attribute.StateChecked }   // checked (on)
			};

			var trackColors = new int[]
			{
				offTrackColor,
				switchView.TrackTintList?.DefaultColor ?? Android.Graphics.Color.Green
			};

			switchView.TrackTintList = new ColorStateList(trackStates, trackColors);
		});
	}
}

#region Usings
#endregion

namespace Mobile;

/// <summary>
/// Aim: Helper class for accessing application resources and theme.
/// </summary>
public static class ResourceHelper
{
	#region Properties
	/// <summary>
	/// Aim: Check if the current theme is dark.
	/// </summary>
	public static bool IsDarkTheme => Application.Current?.RequestedTheme == AppTheme.Dark;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Get a color resource by key.
	/// Params: key (string) - The resource key.
	/// Return (Color): The color resource.
	/// </summary>
	public static Color GetColor(string key)
	{
		var color = (Color)Application.Current!.Resources[key];
		return color;
	}

	/// <summary>
	/// Aim: Get a style resource by key.
	/// Params: key (string) - The resource key.
	/// Return (Style): The style resource.
	/// </summary>
	public static Style GetStyle(string key)
	{
		var style = (Style)Application.Current!.Resources[key];
		return style;
	}

	/// <summary>
	/// Aim: Get a color based on current theme.
	/// Params: lightKey (string) - Resource key for light theme.
	/// Params: darkKey (string) - Resource key for dark theme.
	/// Return (Color): The themed color.
	/// </summary>
	public static Color GetThemedColor(string lightKey, string darkKey)
	{
		var color = IsDarkTheme ? GetColor(darkKey) : GetColor(lightKey);
		return color;
	}
	#endregion
}

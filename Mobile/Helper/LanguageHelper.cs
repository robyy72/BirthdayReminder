namespace Mobile;

#region Usings
using MobileLanguages;
#endregion

/// <summary>
/// Aim: Helper methods for language and localization.
/// </summary>
public static class LanguageHelper
{
	/// <summary>
	/// Aim: Gets localized short month name for a given month number.
	/// Params: month - Month number (1-12)
	/// Return: Localized short month name
	/// </summary>
	public static string GetShortMonthName(int month)
	{
		string result = month switch
		{
			1 => Resources.Month_Short_Jan,
			2 => Resources.Month_Short_Feb,
			3 => Resources.Month_Short_Mar,
			4 => Resources.Month_Short_Apr,
			5 => Resources.Month_Short_May,
			6 => Resources.Month_Short_Jun,
			7 => Resources.Month_Short_Jul,
			8 => Resources.Month_Short_Aug,
			9 => Resources.Month_Short_Sep,
			10 => Resources.Month_Short_Oct,
			11 => Resources.Month_Short_Nov,
			12 => Resources.Month_Short_Dec,
			_ => month.ToString()
		};

		return result;
	}

	/// <summary>
	/// Aim: Gets localized full month name for a given month number.
	/// Params: month - Month number (1-12)
	/// Return: Localized full month name
	/// </summary>
	public static string GetFullMonthName(int month)
	{
		string result = month switch
		{
			1 => Resources.Month_January,
			2 => Resources.Month_February,
			3 => Resources.Month_March,
			4 => Resources.Month_April,
			5 => Resources.Month_May,
			6 => Resources.Month_June,
			7 => Resources.Month_July,
			8 => Resources.Month_August,
			9 => Resources.Month_September,
			10 => Resources.Month_October,
			11 => Resources.Month_November,
			12 => Resources.Month_December,
			_ => month.ToString()
		};

		return result;
	}
}

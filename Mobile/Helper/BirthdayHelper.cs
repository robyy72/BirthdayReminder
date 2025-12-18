namespace Mobile;

/// <summary>
/// Aim: Helper methods for birthday calculations.
/// </summary>
public static class BirthdayHelper
{
	/// <summary>
	/// Aim: Checks if a birth year should be displayed.
	/// Params: year - The birth year to check
	/// Return: True if year should be displayed, false if it's a placeholder year or too old
	/// </summary>
	public static bool ShouldDisplayYear(int year)
	{
		if (year <= 0)
		{
			return false;
		}

		int minYear = DateTime.Today.Year - MobileConstants.MAX_AGE;
		if (year < minYear)
		{
			return false;
		}

		List<int> excludedYears = ParseYearsDisplayNot(MobileConstants.YEARS_DISPLAY_NOT);
		bool result = !excludedYears.Contains(year);
		return result;
	}

	/// <summary>
	/// Aim: Parses the YEARS_DISPLAY_NOT string into a list of integers.
	/// Params: yearsString - Comma-separated string of years (e.g. "1,1604,1900,1904")
	/// Return: List of years as integers
	/// </summary>
	public static List<int> ParseYearsDisplayNot(string yearsString)
	{
		var result = new List<int>();
		if (string.IsNullOrWhiteSpace(yearsString))
		{
			return result;
		}

		string[] parts = yearsString.Split(',');
		foreach (string part in parts)
		{
			if (int.TryParse(part.Trim(), out int year))
			{
				result.Add(year);
			}
		}

		return result;
	}

	/// <summary>
	/// Aim: Calculates the age a person will turn on their next birthday.
	/// Params: birthday - The birthday
	/// Return: Age they will turn (or turned if birthday already passed this year)
	/// </summary>
	public static int GetUpcomingAge(Birthday birthday)
	{
		var today = DateTime.Today;
		int age = today.Year - birthday.Year;

		var birthdayThisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
		if (birthdayThisYear < today)
		{
			age++;
		}

		var result = age;
		return result;
	}

	/// <summary>
	/// Aim: Gets formatted date display string for a birthday.
	/// Params: birthday - The birthday
	/// Return: Formatted date string (e.g. "25. Dez" or "25. Dez 1950")
	/// </summary>
	public static string GetDateDisplay(Birthday birthday)
	{
		string monthName = LanguageHelper.GetShortMonthName(birthday.Month);
		string result;

		if (ShouldDisplayYear(birthday.Year))
		{
			result = $"{birthday.Day}. {monthName} {birthday.Year}";
		}
		else
		{
			result = $"{birthday.Day}. {monthName}";
		}

		return result;
	}

	/// <summary>
	/// Aim: Calculates days until next birthday.
	/// Params: birthday - The birthday, today - Today's date
	/// Return: Number of days until next birthday
	/// </summary>
	public static int GetDaysUntilBirthday(Birthday birthday, DateTime today)
	{
		var thisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
		if (thisYear < today)
		{
			thisYear = thisYear.AddYears(1);
		}
		return (thisYear - today).Days;
	}

	/// <summary>
	/// Aim: Calculates days since last birthday (within 30 days).
	/// Params: birthday - The birthday, today - Today's date
	/// Return: Number of days since birthday (0-30), or 0 if outside range
	/// </summary>
	public static int GetDaysSinceBirthday(Birthday birthday, DateTime today)
	{
		var thisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
		if (thisYear > today)
		{
			thisYear = thisYear.AddYears(-1);
		}
		var days = (today - thisYear).Days;
		return days > 0 && days <= 30 ? days : 0;
	}

	/// <summary>
	/// Aim: Converts a DateTime to a Birthday object.
	/// Params: dateTime - The DateTime to convert
	/// Return: Birthday object with day, month and year
	/// </summary>
	public static Birthday ConvertFromDateTimeToBirthday(DateTime dateTime)
	{
		return new Birthday
		{
			Day = dateTime.Day,
			Month = dateTime.Month,
			Year = dateTime.Year
		};
	}

	/// <summary>
	/// Aim: Converts a Birthday to a DateTime object.
	/// Params: birthday - The Birthday to convert
	/// Return: DateTime object (uses year 1900 if birthday year is 0)
	/// </summary>
	public static DateTime ConvertFromBirthdayToDateTime(Birthday birthday)
	{
		int year = birthday.Year > 0 ? birthday.Year : 1900;
		return new DateTime(year, birthday.Month, birthday.Day);
	}
}

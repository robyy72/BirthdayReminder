namespace Mobile;

/// <summary>
/// Aim: Helper methods for birthday calculations.
/// </summary>
public static class BirthdayHelper
{
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

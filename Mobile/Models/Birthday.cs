namespace Mobile;

/// <summary>
/// Aim: Represents a birthday with day, month and year.
/// </summary>
public class Birthday
{
	public int Day { get; set; }
	public int Month { get; set; }
	public int Year { get; set; }

	public Birthday()
	{
	}

	public Birthday(int day, int month, int year)
	{
		Day = day;
		Month = month;
		Year = year;
	}
}

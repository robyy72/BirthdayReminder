namespace Mobile;

#region Usings
using Common;
#endregion

/// <summary>
/// Aim: ViewModel for displaying a person in lists.
/// </summary>
public class PersonViewModel
{
	public int Id { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string DayDisplay { get; set; } = string.Empty;
	public string MonthDisplay { get; set; } = string.Empty;
	public string DateDisplay { get; set; } = string.Empty;
	public string AgeDisplay { get; set; } = string.Empty;
	public bool HasYear { get; set; }
	public int BirthdayMonth { get; set; }

	public PersonViewModel(Person person, PersonNameDirection nameDirection)
	{
		Id = person.Id;
		FirstName = person.FirstName;
		LastName = person.LastName;
		DisplayName = PersonHelper.GetDisplayName(person.FirstName, person.LastName, nameDirection);

		if (person.Birthday != null)
		{
			BirthdayMonth = person.Birthday.Month;
			DayDisplay = $"{person.Birthday.Day}.";
			MonthDisplay = LanguageHelper.GetShortMonthName(person.Birthday.Month);
			HasYear = BirthdayHelper.ShouldDisplayYear(person.Birthday.Year);
			DateDisplay = BirthdayHelper.GetDateDisplay(person.Birthday);

			if (HasYear)
			{
				int age = BirthdayHelper.GetUpcomingAge(person.Birthday);
				AgeDisplay = LanguageHelper.GetAgeDisplay(age);
			}
			else
			{
				AgeDisplay = "-";
			}
		}
	}
}

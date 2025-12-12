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
	public string DayDisplay { get; set; } = string.Empty;
	public string MonthDisplay { get; set; } = string.Empty;

	public PersonViewModel(Person person, PersonNameDirection nameDirection)
	{
		Id = person.Id;
		DisplayName = PersonHelper.GetDisplayName(person.FirstName, person.LastName, nameDirection);
		if (person.Birthday != null)
		{
			DayDisplay = $"{person.Birthday.Day}.";
			MonthDisplay = LanguageHelper.GetShortMonthName(person.Birthday.Month);
		}
	}
}

namespace Mobile;

/// <summary>
/// Aim: ViewModel for displaying a person in lists.
/// </summary>
public class PersonViewModel
{
	public int Id { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public string BirthdayDisplay { get; set; } = string.Empty;

	public PersonViewModel(Person person)
	{
		Id = person.Id;
		DisplayName = person.DisplayName;
		if (person.Birthday != null)
		{
			BirthdayDisplay = $"{person.Birthday.Day:00}.{person.Birthday.Month:00}.{person.Birthday.Year}";
		}
	}
}

namespace Mobile;

/// <summary>
/// Aim: Represents a person with name and birthday.
/// </summary>
public class Person
{
	public int Id { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public Birthday? Birthday { get; set; }
	public int ReminderTime { get; set; } = MobileConstants.DEFAULT_REMINDER_TIME;
	public ReminderType ReminderType { get; set; } = ReminderType.NotSet;
	public bool NeverReadFromContacts { get; set; }
	public bool NeverWriteToContacts { get; set; }
	public string? ContactId { get; set; }
	public PersonSource Source { get; set; } = PersonSource.Manual;
}

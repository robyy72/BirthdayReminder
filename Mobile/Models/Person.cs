namespace Mobile;

/// <summary>
/// Aim: Represents a person with name and birthday.
/// </summary>
public class Person
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public Birthday? Birthday { get; set; }
	public int ReminderTime { get; set; } = MobileConstants.DEFAULT_REMINDER_TIME;
	public ReminderType ReminderType { get; set; } = ReminderType.NotSet;
	public bool NeverReadFromContacts { get; set; }
	public bool NeverWriteToContacts { get; set; }

	public Person()
	{
	}

	public Person(int id, string name, Birthday? birthday, int reminderTime, ReminderType reminderType, bool neverReadFromContacts, bool neverWriteToContacts)
	{
		Id = id;
		Name = name;
		Birthday = birthday;
		ReminderTime = reminderTime;
		ReminderType = reminderType;
		NeverReadFromContacts = neverReadFromContacts;
		NeverWriteToContacts = neverWriteToContacts;
	}
}

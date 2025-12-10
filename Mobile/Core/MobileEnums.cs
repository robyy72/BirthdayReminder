namespace Mobile;

public enum ReminderType
{
	NotSet = 0,
	DoNotRemind = 1,
	RemindByMessage = 2,
	RemindUntilApproved = 3
}

public enum ContactsMode
{
	Read = 0,
	ReadWrite = 1,
	BirthdayCalendar = 2,
	None = 3
}

public enum PersonSource
{
	Manual = 0,
	Contact = 1,
	BirthdayCalendar = 2
}

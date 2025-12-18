namespace Mobile;

public enum HelpTopic
{
	UseContacts = 1,
	ReadFromBirthdayCalendar = 2,
	BirthdayWithoutYear = 3
}

public enum ContactsMode
{
	NotSet = 0,
	None = 1,
	ReadFromContacts = 2,
	BirthdayCalendar = 3	
}

public enum PersonSource
{
	Manual = 0,
	Contacts = 1,
	BirthdayCalendar = 2
}

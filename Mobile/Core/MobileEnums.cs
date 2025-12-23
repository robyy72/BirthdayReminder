namespace Mobile;

public enum HelpTopic
{
	UseContacts = 1,
	BirthdayWithoutYear = 2
}

public enum ContactsReadMode
{
	NotSet = 0,
	None = 1,
	ReadNamesWithBirthday = 2,
	ReadAllNames = 3
}

public enum ContactsReadWriteMode
{
	NotSet = 0,
	ReadOnlyOnce = 1,
	ReadAlways = 2,
	ReadAlwaysAndAskWriteBack = 3,
	ReadAlwaysAndWriteBack = 4
}

// at the moment only used for iOS
public enum ContactsAccessChoice
{
	NotSet = 0,
	OnlySomeContacts = 1,
    AllContacts = 2,
}

public enum PersonSource
{
	Manual = 0,
	Contacts = 1
}

public enum ReminderCount
{
	NoReminder = 0,
	OneReminder = 1,
	TwoReminders = 2,
	ThreeReminders = 3
}

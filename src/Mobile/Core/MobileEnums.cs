namespace Mobile;

public enum HelpTopic
{
	UseContacts = 1,
	BirthdayWithoutYear = 2,
	FreeReminders = 3,
	ReminderNotification = 4,
	ReminderAlarm = 5,
	ReminderEmail = 6,
	ReminderSignal = 7,
	ReminderSms = 8,
	ReminderWhatsApp = 9,
	Timezone = 10
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

public enum AppPermissionStatus
{
	NotSet = 0,
	Granted = 1,
	Denied = 2
}

public enum PermissionType
{
	NotSet = 0,
	Contacts = 1,
	Calendar = 2
}

public enum PurchaseResult
{
	Success = 0,
	Cancelled = 1,
	Error = 2,
	AlreadyOwned = 3
}

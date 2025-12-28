namespace Common;

public enum PersonNameDirection
{
	NotSet = 0,
	FirstFirstName = 1,
	FirstLastName = 2,
	FirstNameOnly = 3
}

public enum SupportEntryType	
{
    NotSet = 0,
	Error = 1,
	SupportRequest = 2,
    FeatureRequest = 3
}

public enum SupportEntryStatus
{
	Created = 1,
	Assigned = 2,
	WeAnswered = 3,
    WaitingForClientAnswer = 4,
    Successful = 5,
	Cancelled = 6
}

public enum SubscriptionTier
{
	Free,
	Plus,
	Pro
}

#region Reminder Method Types
public enum LocalMethodType
{
	Notification,
	Alarm
}

public enum ExternalMethodType
{
	Email,
	Sms,
	WhatsApp,
	Signal
}

public enum NotificationPriority
{
	Low,
	Normal,
	High,
	Critical
}
#endregion

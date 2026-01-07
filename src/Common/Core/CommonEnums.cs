namespace Common;

#region App and Subscription
public enum AppStore
{
	Unknown = 0,
	GooglePlayStore = 1,
	AppleAppStore = 2
}

public enum SubscriptionTier
{
	Free,
	Plus,
	Pro
}
#endregion

#region Customer
public enum PreferredChannel
{
	NotSet = 0,
	Email = 1,
	Sms = 2,
	WhatsApp = 3,
	Signal = 4
}

public enum PersonNameDirection
{
	NotSet = 0,
	FirstFirstName = 1,
	FirstLastName = 2,
	FirstNameOnly = 3
}
#endregion

#region Tickets
public enum TicketType
{
	NotSet = 0,
	Error = 1,
	SupportRequest = 2,
	FeatureRequest = 3,
	CustomerFeedback = 4
}

public enum TicketSource
{
	NotSet = 0,
	FromCustomerViaApp = 1,
	FromCustomerViaEmail = 2,
	ManuallyByAdmin = 3
}

public enum TicketStatus
{
	Created = 1,
	Assigned = 2,
	WeAnswered = 3,
	WaitingForClientAnswer = 4,
	Successful = 5,
	Cancelled = 6
}
#endregion

#region Reminders
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

#region Infrastructure
public enum DatabaseProviderType
{
	NotSet = 0,
	MsSqlServer = 1,
	Oracle = 2
}
#endregion

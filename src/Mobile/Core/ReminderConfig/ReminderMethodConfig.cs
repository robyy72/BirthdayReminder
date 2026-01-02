#region Usings
using Common;
using MobileLanguages;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Central configuration for all reminder methods.
/// Contains default values, icons, and metadata for each method type.
/// </summary>
public static class ReminderMethodConfig
{
	#region Local Methods
	public static readonly Dictionary<LocalMethodType, LocalMethodInfo> Local = new()
	{
		[LocalMethodType.Notification] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_LOCKSCREEN,
			Icon: "icon_notification.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_Notification),
			DefaultPriority: NotificationPriority.Normal,
			DefaultSound: CommonConstants.DEFAULT_NOTIFICATION_SOUND,
			DefaultVibrate: CommonConstants.DEFAULT_NOTIFICATION_VIBRATE,
			DefaultWakeScreen: CommonConstants.DEFAULT_NOTIFICATION_WAKE_SCREEN,
			DefaultOverrideSilentMode: CommonConstants.DEFAULT_NOTIFICATION_OVERRIDE_SILENT
		),
		[LocalMethodType.Alarm] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_LOCKSCREEN,
			Icon: "icon_alarm.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_Alarm),
			DefaultPriority: NotificationPriority.High,
			DefaultSound: CommonConstants.DEFAULT_ALARM_SOUND,
			DefaultVibrate: CommonConstants.DEFAULT_ALARM_VIBRATE,
			DefaultWakeScreen: CommonConstants.DEFAULT_ALARM_WAKE_SCREEN,
			DefaultOverrideSilentMode: CommonConstants.DEFAULT_ALARM_OVERRIDE_SILENT
		),
	};
	#endregion

	#region External Methods
	public static readonly Dictionary<ExternalMethodType, ExternalMethodInfo> External = new()
	{
		[ExternalMethodType.Email] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_EMAIL,
			Icon: "icon_email.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_Email),
			TemplateKeyToday: nameof(Resources.ReminderTemplate_Email_Today),
			TemplateKeyTomorrow: nameof(Resources.ReminderTemplate_Email_Tomorrow),
			TemplateKeyInDays: nameof(Resources.ReminderTemplate_Email_InDays)
		),
		[ExternalMethodType.Sms] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_SMS,
			Icon: "icon_sms.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_Sms),
			TemplateKeyToday: nameof(Resources.ReminderTemplate_Sms_Today),
			TemplateKeyTomorrow: nameof(Resources.ReminderTemplate_Sms_Tomorrow),
			TemplateKeyInDays: nameof(Resources.ReminderTemplate_Sms_InDays)
		),
		[ExternalMethodType.WhatsApp] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_WHATSAPP,
			Icon: "icon_whatsapp.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_WhatsApp),
			TemplateKeyToday: nameof(Resources.ReminderTemplate_WhatsApp_Today),
			TemplateKeyTomorrow: nameof(Resources.ReminderTemplate_WhatsApp_Tomorrow),
			TemplateKeyInDays: nameof(Resources.ReminderTemplate_WhatsApp_InDays)
		),
		[ExternalMethodType.Signal] = new(
			IsFree: true,
			DefaultTime: CommonConstants.DEFAULT_REMINDER_TIME_SIGNAL,
			Icon: "icon_signal.png",
			DisplayNameKey: nameof(Resources.ReminderMethod_Signal),
			TemplateKeyToday: nameof(Resources.ReminderTemplate_Signal_Today),
			TemplateKeyTomorrow: nameof(Resources.ReminderTemplate_Signal_Tomorrow),
			TemplateKeyInDays: nameof(Resources.ReminderTemplate_Signal_InDays)
		),
	};
	#endregion

	#region Version Arrays
	/// <summary>
	/// Methods available in the free version.
	/// </summary>
	public static readonly LocalMethodType[] FreeVersionLocalMethods =
	[
		LocalMethodType.Notification,
		LocalMethodType.Alarm
	];

	public static readonly ExternalMethodType[] FreeVersionExternalMethods =
	[
		ExternalMethodType.Email,
		ExternalMethodType.Sms,
		ExternalMethodType.WhatsApp,
		ExternalMethodType.Signal
	];

	/// <summary>
	/// Additional methods available only in the pro version.
	/// </summary>
	public static readonly LocalMethodType[] ProVersionLocalMethods =
	[
		// Currently none
	];

	public static readonly ExternalMethodType[] ProVersionExternalMethods =
	[
		// Currently none
	];
	#endregion

	#region Helper Methods
	public static bool IsMethodAvailableInFreeVersion(LocalMethodType type) =>
		FreeVersionLocalMethods.Contains(type);

	public static bool IsMethodAvailableInFreeVersion(ExternalMethodType type) =>
		FreeVersionExternalMethods.Contains(type);

	public static bool IsMethodAvailableInProVersion(LocalMethodType type) =>
		FreeVersionLocalMethods.Contains(type) || ProVersionLocalMethods.Contains(type);

	public static bool IsMethodAvailableInProVersion(ExternalMethodType type) =>
		FreeVersionExternalMethods.Contains(type) || ProVersionExternalMethods.Contains(type);

	public static IEnumerable<LocalMethodType> GetAvailableLocalMethods(bool isPro) =>
		isPro
			? FreeVersionLocalMethods.Concat(ProVersionLocalMethods)
			: FreeVersionLocalMethods;

	public static IEnumerable<ExternalMethodType> GetAvailableExternalMethods(bool isPro) =>
		isPro
			? FreeVersionExternalMethods.Concat(ProVersionExternalMethods)
			: FreeVersionExternalMethods;
	#endregion

	#region Template Helpers
	public static string GetAgeText(string name, int age) =>
		string.Format(Resources.ReminderTemplate_AgeText, name, age)
			.Replace("{Name}", name)
			.Replace("{Age}", age.ToString());

	public static string GetGoodbye() =>
		Resources.ReminderTemplate_Goodbye;

	public static string GetTeamSignature() =>
		Resources.ReminderTemplate_TeamSignature.Replace("{Domain}", CommonConstants.DOMAIN);
	#endregion
}

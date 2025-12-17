#region Usings
using Common;
using Plugin.LocalNotification;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for scheduling and managing birthday reminder notifications.
/// </summary>
public static class NotificationService
{
	/// <summary>
	/// Aim: Schedule a notification for a person's birthday.
	/// Params: person - The person to schedule notification for.
	/// </summary>
	public static void ScheduleForPerson(Person person)
	{
		if (person.Birthday == null)
			return;

		if (!HasAnyReminderEnabled(person))
			return;

		CancelForPerson(person.Id);

		var settings = SettingsService.Get();
		int reminderTime = GetEarliestReminderTime(person, settings);
		int hours = reminderTime / 100;
		int minutes = reminderTime % 100;

		var nextBirthday = GetNextBirthdayDate(person.Birthday, hours, minutes);
		if (nextBirthday <= DateTime.Now)
			return;

		var displayName = GetDisplayName(person, settings);
		var title = MobileLanguages.Resources.General_Label_Birthday;
		var description = string.Format("{0}", displayName);

		var request = new NotificationRequest
		{
			NotificationId = person.Id,
			Title = title,
			Description = description,
			Schedule = new NotificationRequestSchedule
			{
				NotifyTime = nextBirthday,
				RepeatType = NotificationRepeat.No
			}
		};

		LocalNotificationCenter.Current.Show(request);
	}

	/// <summary>
	/// Aim: Cancel a scheduled notification for a person.
	/// Params: personId - The person's ID.
	/// </summary>
	public static void CancelForPerson(int personId)
	{
		LocalNotificationCenter.Current.Cancel(personId);
	}

	/// <summary>
	/// Aim: Schedule notifications for all persons with reminders enabled.
	/// </summary>
	public static void ScheduleAll()
	{
		for (int month = 1; month <= 12; month++)
		{
			var persons = BirthdayService.GetByMonth(month);
			foreach (var person in persons)
			{
				ScheduleForPerson(person);
			}
		}
	}

	/// <summary>
	/// Aim: Cancel all scheduled notifications.
	/// </summary>
	public static void CancelAll()
	{
		LocalNotificationCenter.Current.CancelAll();
	}

	/// <summary>
	/// Aim: Request notification permission from the user.
	/// Return: True if granted, false otherwise.
	/// </summary>
	public static async Task<bool> RequestPermissionAsync()
	{
		var result = await LocalNotificationCenter.Current.RequestNotificationPermission();
		return result;
	}

	private static bool HasAnyReminderEnabled(Person person)
	{
		bool result = person.ReminderEmailEnabled ||
					  person.ReminderSmsEnabled ||
					  person.ReminderLockScreenEnabled ||
					  person.ReminderWhatsAppEnabled;
		return result;
	}

	private static int GetEarliestReminderTime(Person person, Settings settings)
	{
		var times = new List<int>();

		if (person.ReminderEmailEnabled)
			times.Add(settings.ReminderTimeEmail);
		if (person.ReminderSmsEnabled)
			times.Add(settings.ReminderTimeSms);
		if (person.ReminderLockScreenEnabled)
			times.Add(settings.ReminderTimeLockScreen);
		if (person.ReminderWhatsAppEnabled)
			times.Add(settings.ReminderTimeWhatsApp);

		int result = times.Count > 0 ? times.Min() : CommonConstants.DEFAULT_REMINDER_TIME_MORNING;
		return result;
	}

	private static DateTime GetNextBirthdayDate(Birthday birthday, int hours, int minutes)
	{
		var today = DateTime.Today;
		var thisYearBirthday = new DateTime(today.Year, birthday.Month, birthday.Day, hours, minutes, 0);

		if (thisYearBirthday < DateTime.Now)
		{
			thisYearBirthday = thisYearBirthday.AddYears(1);
		}

		return thisYearBirthday;
	}

	private static string GetDisplayName(Person person, Settings settings)
	{
		if (settings.PersonNameDirection == PersonNameDirection.FirstLastName)
		{
			return $"{person.LastName} {person.FirstName}".Trim();
		}

		return $"{person.FirstName} {person.LastName}".Trim();
	}
}

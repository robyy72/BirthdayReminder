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

		if (person.ReminderType == ReminderType.NotSet || person.ReminderType == ReminderType.DoNotRemind)
			return;

		CancelForPerson(person.Id);

		var settings = SettingsService.Get();
		int hours = settings.DefaultReminderTime / 100;
		int minutes = settings.DefaultReminderTime % 100;

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

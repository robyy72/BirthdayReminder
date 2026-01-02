#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for managing reminders.
/// </summary>
public static class ReminderService
{
	/// <summary>
	/// Aim: Counts the number of non-null reminders for a person.
	/// Params: person - The person to count reminders for
	/// Return: Number of used reminders (0-3)
	/// </summary>
	public static int CountUsedReminders(Person person)
	{
		int count = person.Reminders.Count(r => r != null);
		return count;
	}

	/// <summary>
	/// Aim: Counts the number of non-null template reminders.
	/// Return: Number of used template reminders (0-3)
	/// </summary>
	public static int CountUsedTemplateReminders()
	{
		int count = App.Account.ReminderTemplates.Count(r => r != null);
		return count;
	}

	/// <summary>
	/// Aim: Gets all non-null reminders sorted by DaysBefore descending.
	/// Params: person - The person to get reminders for
	/// Return: List of reminders sorted by days (largest first)
	/// </summary>
	public static List<Reminder> GetSortedReminders(Person person)
	{
		var result = person.Reminders
			.Where(r => r != null)
			.Cast<Reminder>()
			.OrderByDescending(r => r.DaysBefore)
			.ToList();

		return result;
	}

	/// <summary>
	/// Aim: Gets all non-null template reminders sorted by DaysBefore descending.
	/// Return: List of template reminders sorted by days (largest first)
	/// </summary>
	public static List<Reminder> GetSortedTemplateReminders()
	{
		var result = App.Account.ReminderTemplates
			.Where(r => r != null)
			.Cast<Reminder>()
			.OrderByDescending(r => r.DaysBefore)
			.ToList();

		return result;
	}

	/// <summary>
	/// Aim: Gets the days already used by other reminders (excluding the current index).
	/// Params: person - The person to check, excludeIndex - Index to exclude from check
	/// Return: Set of days already in use
	/// </summary>
	public static HashSet<int> GetUsedDays(Person? person, int excludeIndex)
	{
		var usedDays = new HashSet<int>();
		var reminders = person?.Reminders ?? App.Account.ReminderTemplates;

		for (int i = 0; i < reminders.Length; i++)
		{
			if (i != excludeIndex && reminders[i] != null)
			{
				usedDays.Add(reminders[i]!.DaysBefore);
			}
		}

		return usedDays;
	}
}

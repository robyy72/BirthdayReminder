#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a person with name and birthday.
/// </summary>
public class Person
{
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public Birthday? Birthday { get; set; }
	public int ReminderTime { get; set; } = CommonConstants.DEFAULT_REMINDER_TIME_MORNING;
	public ReminderMethod ReminderMethod { get; set; } = ReminderMethod.NotSet;
	public bool RemindUntilApproved { get; set; }
	public bool NeverReadFromContacts { get; set; }
	public bool NeverWriteToContacts { get; set; }
	public string? ContactId { get; set; }
	public PersonSource Source { get; set; } = PersonSource.Manual;
}

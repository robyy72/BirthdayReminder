#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a person with name and birthday.
/// </summary>
public class Person
{
	#region Identity
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public Birthday? Birthday { get; set; }
    #endregion

    #region Reminder
    public bool ReminderEmailEnabled { get; set; }
    public bool ReminderSmsEnabled { get; set; }
    public bool ReminderLockScreenEnabled { get; set; }
    public bool ReminderWhatsAppEnabled { get; set; }
    public bool RemindUntilApproved { get; set; }
	#endregion

	#region Contacts
	public string? ContactId { get; set; }
	public PersonSource Source { get; set; } = PersonSource.Manual;
	#endregion
}

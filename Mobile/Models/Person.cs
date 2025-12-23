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

    #region Reminders
    public Reminder? Reminder_1 { get; set; }
    public Reminder? Reminder_2 { get; set; }
    public Reminder? Reminder_3 { get; set; }
	#endregion

	#region Contacts
	public string? ContactId { get; set; }
	public PersonSource Source { get; set; } = PersonSource.Manual;
	public ContactsReadMode ContactsReadMode { get; set; } = ContactsReadMode.NotSet;
	public ContactsReadWriteMode ContactsReadWriteMode { get; set; } = ContactsReadWriteMode.NotSet;
	#endregion
}

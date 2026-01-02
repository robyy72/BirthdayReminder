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

	/// <summary>
	/// Aim: Gets the formatted display name based on current settings.
	/// Return: Display name in format "FirstName LastName" or "LastName, FirstName"
	/// </summary>
	public string DisplayName => PersonHelper.GetDisplayName(FirstName, LastName, App.Account.PersonNameDirection);
    #endregion

    #region Reminders
    public Reminder?[] Reminders { get; set; } = new Reminder?[MobileConstants.MAX_REMINDERS];
	#endregion

	#region Contacts
	public string? ContactId { get; set; }
	public PersonSource Source { get; set; } = PersonSource.Manual;
	public ContactsReadMode ContactsReadMode { get; set; } = ContactsReadMode.NotSet;
	public ContactsReadWriteMode ContactsReadWriteMode { get; set; } = ContactsReadWriteMode.NotSet;
	#endregion
}

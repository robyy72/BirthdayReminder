namespace Mobile;

/// <summary>
/// Aim: Platform-agnostic service for reading contact birthdays.
/// </summary>
public partial class ContactBirthdayService
{
	/// <summary>
	/// Aim: Gets contacts from the device.
	/// Params: onlyWithBirthday - If true, only returns contacts that have a birthday set
	/// Return: List of Person objects with FirstName, LastName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync(bool onlyWithBirthday);
}

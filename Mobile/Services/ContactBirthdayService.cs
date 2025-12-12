namespace Mobile;

/// <summary>
/// Aim: Platform-agnostic service for reading contact birthdays.
/// </summary>
public partial class ContactBirthdayService
{
	/// <summary>
	/// Aim: Gets all contacts with birthdays from the device.
	/// Return: List of Person objects with DisplayName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync();

	/// <summary>
	/// Aim: Gets all birthdays from the device birthday calendar.
	/// Return: List of Person objects with DisplayName and Birthday
	/// </summary>
	public partial Task<List<Person>> GetBirthdayCalendarEventsAsync();
}

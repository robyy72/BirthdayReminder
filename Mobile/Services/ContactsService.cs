namespace Mobile;

/// <summary>
/// Aim: Platform-agnostic service for reading contacts.
/// </summary>
public partial class ContactsService
{
	/// <summary>
	/// Aim: Gets contacts from the device.
	/// Params: onlyWithBirthday - If true, only returns contacts that have a birthday set
	/// Return: List of Person objects with FirstName, LastName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync(bool onlyWithBirthday);

	/// <summary>
	/// Aim: Reads contacts into App.Contacts if allowed. If Persons is empty, copies contacts to Persons.
	/// </summary>
	public static async void ReadContactsIfAllowed()
	{
		if (!AccountService.UseContacts())
			return;

		try
		{
			var service = new ContactsService();
			bool onlyWithBirthday = App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday;
			App.Contacts = await service.GetContactsAsync(onlyWithBirthday);

			// If no persons exist yet, import contacts as persons
			if (App.Persons.Count == 0 && App.Contacts.Count > 0)
			{
				foreach (var contact in App.Contacts)
				{
					if (string.IsNullOrWhiteSpace(contact.FirstName) && string.IsNullOrWhiteSpace(contact.LastName))
						continue;

					contact.Source = PersonSource.Contacts;
					PersonService.Add(contact);
				}
			}

			// TODO: Search / compare logic for when Persons already exist
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading contacts: {ex.Message}");
		}
	}
}

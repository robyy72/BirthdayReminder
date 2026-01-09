using Common;

namespace Mobile;

/// <summary>
/// Aim: Platform-agnostic service for reading contacts.
/// A lot of platform-specific code is in the partial classes (check Platform folder)
/// </summary>
public partial class ContactsService
{
	/// <summary>
	/// Aim: Gets contacts from the device.
	/// Params: onlyWithBirthday - If true, only returns contacts that have a birthday set
	/// Return: List of Contact objects with FirstName, LastName, Birthday and Id
	/// </summary>
	public partial Task<List<Contact>> GetContactsAsync(bool onlyWithBirthday);

	/// <summary>
	/// Aim: Reads contacts into App.Contacts if allowed. If Persons is empty, copies contacts to Persons.
	/// </summary>
	public static async Task ReadContactsIfAllowedAsync()
	{
		if (!AccountService.UseContacts())
			return;

		try
		{
			var serviceWithPlatformCode = new ContactsService();
			bool onlyWithBirthday = App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday;
			App.Contacts = await serviceWithPlatformCode.GetContactsAsync(onlyWithBirthday);

			// no work with no contacts found
			if (App.Contacts.Count == 0) return;

            // If no persons exist yet, import contacts as persons
            if (App.Persons.Count == 0)
			{
				foreach (Contact contact in App.Contacts)
				{
					if (string.IsNullOrWhiteSpace(contact.FirstName) && string.IsNullOrWhiteSpace(contact.LastName))
						continue;

					Person person = ConvertContactToPerson(contact);
					person.Source = PersonSource.Contacts;
					PersonService.Add(person);
				}

				// set the PersonNameDirection and store it into the Account
				App.Account.PersonNameDirection = GetDeviceNameOrder();

				// Default first FirstName 
				if (App.Account.PersonNameDirection == PersonNameDirection.NotSet)
					App.Account.PersonNameDirection = PersonNameDirection.FirstFirstName;

				AccountService.Save();
            }

			// TODO: Search / compare logic for when Persons already exist
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading contacts: {ex.Message}");
		}
	}

	public static partial Person ConvertContactToPerson(Contact contact);
	public static partial Contact ConvertPersonToContact(Person person);
	public static partial PersonNameDirection GetDeviceNameOrder();
}

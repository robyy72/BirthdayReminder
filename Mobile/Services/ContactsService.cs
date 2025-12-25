using Common;

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
			var serviceWithPlatformCode = new ContactsService();
			bool onlyWithBirthday = App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday;
			App.Contacts = await serviceWithPlatformCode.GetContactsAsync(onlyWithBirthday);

			// no work with no contacts found
			if (App.Contacts.Count == 0) return;

            // If no persons exist yet, import contacts as persons
            if (App.Persons.Count == 0)
			{
				List<Contact> contacts = App.Contacts;

				int countDisplayNamesWithCommaForAndroid = 0;
				foreach (var contact in App.Contacts)
				{
					if (string.IsNullOrWhiteSpace(contact.FirstName) && string.IsNullOrWhiteSpace(contact.LastName))
						continue;

					contact.Source = PersonSource.Contacts;
					PersonService.Add(contact);

					// count Commata for Android Contacts
					if (App.DeviceSystem == DeviceSystem.Android)
						if (contact.DisplayName.Contains(","))
							countDisplayNamesWithCommaForAndroid++;

					// remove from the relevant list because it's added
					App.Contacts.Remove(contact);
                }

				// set the PersonNameDirection and store it into the Account
				switch (App.DeviceSystem)
				{
					case DeviceSystem.iOS:
#if IOS
						var contact = new Contacts.CNContact();
						var nameOrder = Contacts.CNContactFormatter.GetNameOrderForContact(contact);
						App.Account.PersonNameDirection = nameOrder == Contacts.CNContactDisplayNameOrder.GivenNameFirst
							? PersonNameDirection.FirstFirstName
							: PersonNameDirection.FirstLastName;
#endif
                        break;

					case DeviceSystem.Android:
						// more than 50% DisplayNames have Comma ?
						if (countDisplayNamesWithCommaForAndroid > Math.Abs(App.Contacts.Count / 2))
							App.Account.PersonNameDirection = PersonNameDirection.FirstLastName;
						else
							App.Account.PersonNameDirection = PersonNameDirection.FirstFirstName;
						break;
                }

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
}

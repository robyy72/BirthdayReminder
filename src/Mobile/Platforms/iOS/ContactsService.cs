#region Usings
using Common;
using Contacts;
using EventKit;
using Foundation;
#endregion

namespace Mobile;

/// <summary>
/// Aim: iOS implementation for reading contacts.
/// </summary>
public partial class ContactsService
{
	/// <summary>
	/// Aim: Gets contacts from iOS contacts.
	/// Params: onlyWithBirthday - If true, only returns contacts that have a birthday set
	/// Return: List of Contact objects with FirstName, LastName, BirthdayAsDateTime and Id
	/// </summary>
	public partial Task<List<Contact>> GetContactsAsync(bool onlyWithBirthday)
	{
		var results = new List<Contact>();

		try
		{
			var store = new CNContactStore();
			var keysToFetch = new NSString[]
			{
				CNContactKey.Identifier,
				CNContactKey.GivenName,
				CNContactKey.FamilyName,
				CNContactKey.Birthday
			};

			// Use predicate to fetch all contacts
			var containers = store.GetContainers(null, out var containerError);
			if (containerError != null || containers == null)
			{
				System.Diagnostics.Debug.WriteLine($"Error fetching containers: {containerError?.LocalizedDescription}");
				return Task.FromResult(results);
			}

			int idCounter = 1;
			foreach (var container in containers)
			{
				var predicate = CNContact.GetPredicateForContactsInContainer(container.Identifier);
				var contacts = store.GetUnifiedContacts(predicate, keysToFetch, out var fetchError);

				if (fetchError != null)
				{
					System.Diagnostics.Debug.WriteLine($"Error fetching contacts: {fetchError.LocalizedDescription}");
					continue;
				}

				if (contacts == null)
					continue;

				foreach (var cnContact in contacts)
				{
					// Skip contacts without birthday if filter is enabled
					if (onlyWithBirthday && cnContact.Birthday == null)
						continue;

					string firstName = cnContact.GivenName ?? string.Empty;
					string lastName = cnContact.FamilyName ?? string.Empty;
					if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
						continue;

					Birthday? birthday = null;
					if (cnContact.Birthday != null)
					{
						var birthdayComponents = cnContact.Birthday;
						int day = (int)birthdayComponents.Day;
						int month = (int)birthdayComponents.Month;
						int year = birthdayComponents.Year > 0 ? (int)birthdayComponents.Year : 0;

						if (day > 0 && day <= 31 && month > 0 && month <= 12)
						{
							birthday = new Birthday { Day = day, Month = month, Year = year };
						}
					}

					var contact = new Contact
					{
						Id = idCounter++,
						FirstName = firstName,
						LastName = lastName,
						DisplayName = $"{firstName} {lastName}".Trim(),
						Birthday = birthday
					};

					results.Add(contact);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading iOS contacts: {ex.Message}");
		}

		return Task.FromResult(results);
	}

	public static partial Person ConvertContactToPerson(Contact contact) => new()
	{
		FirstName = contact.FirstName,
		LastName = contact.LastName,
		Birthday = contact.Birthday,
		ContactId = contact.Id.ToString()
	};

	public static partial Contact ConvertPersonToContact(Person person) => new()
	{
		FirstName = person.FirstName,
		LastName = person.LastName,
		Birthday = person.Birthday,
		DisplayName = $"{person.FirstName} {person.LastName}".Trim()
	};

	public static partial PersonNameDirection GetDeviceNameOrder()
	{
		var cnContact = new CNContact();
		var nameOrder = CNContactFormatter.GetNameOrderFor(cnContact);

		PersonNameDirection result = nameOrder == CNContactDisplayNameOrder.GivenNameFirst
			? PersonNameDirection.FirstFirstName
			: PersonNameDirection.FirstLastName;

		return result;
	}

	/// <summary>
	/// Aim: Updates a contact's birthday in iOS contacts.
	/// Params: contactId - Contact identifier, birthday - Birthday to set
	/// Return: True if update was successful
	/// </summary>
	public partial Task<bool> UpdateContactBirthdayAsync(string contactId, Birthday birthday)
	{
		try
		{
			var store = new CNContactStore();

			// Find the contact by identifier
			var keysToFetch = new NSString[]
			{
				CNContactKey.Identifier,
				CNContactKey.Birthday
			};

			var contact = store.GetUnifiedContact(contactId, keysToFetch, out var fetchError);
			if (fetchError != null || contact == null)
			{
				System.Diagnostics.Debug.WriteLine($"Error fetching contact: {fetchError?.LocalizedDescription}");
				return Task.FromResult(false);
			}

			// Create mutable copy
			var mutableContact = contact.MutableCopy() as CNMutableContact;
			if (mutableContact == null)
				return Task.FromResult(false);

			// Create birthday components
			var birthdayComponents = new NSDateComponents
			{
				Day = birthday.Day,
				Month = birthday.Month
			};

			if (birthday.Year > 0)
			{
				birthdayComponents.Year = birthday.Year;
			}

			mutableContact.Birthday = birthdayComponents;

			// Save the contact
			var saveRequest = new CNSaveRequest();
			saveRequest.UpdateContact(mutableContact);

			bool success = store.ExecuteSaveRequest(saveRequest, out var saveError);
			if (saveError != null)
			{
				System.Diagnostics.Debug.WriteLine($"Error saving contact: {saveError.LocalizedDescription}");
				return Task.FromResult(false);
			}

			return Task.FromResult(success);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error updating iOS contact birthday: {ex.Message}");
			return Task.FromResult(false);
		}
	}
}

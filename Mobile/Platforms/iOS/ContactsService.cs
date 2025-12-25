#region Usings
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
	/// Return: List of Person objects with FirstName, LastName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync(bool onlyWithBirthday)
	{
		var results = new List<Person>();

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

				foreach (var contact in contacts)
				{
					// Skip contacts without birthday if filter is enabled
					if (onlyWithBirthday && contact.Birthday == null)
						continue;

					string? contactId = contact.Identifier;
					if (string.IsNullOrWhiteSpace(contactId))
						continue;

					string firstName = contact.GivenName ?? string.Empty;
					string lastName = contact.FamilyName ?? string.Empty;
					if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
						continue;

					Birthday? birthday = null;
					if (contact.Birthday != null)
					{
						var birthdayComponents = contact.Birthday;
						int day = (int)birthdayComponents.Day;
						int month = (int)birthdayComponents.Month;
						int year = birthdayComponents.Year > 0 ? (int)birthdayComponents.Year : 0;

						if (day > 0 && day <= 31 && month > 0 && month <= 12)
						{
							birthday = new Birthday { Day = day, Month = month, Year = year };
						}
					}

					var person = new Person
					{
						FirstName = firstName,
						LastName = lastName,
						Birthday = birthday,
						ContactId = contactId,
						Source = PersonSource.Contacts
					};

					results.Add(person);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading iOS contacts: {ex.Message}");
		}

		return Task.FromResult(results);
	}

}

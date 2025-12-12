#region Usings
using Contacts;
using EventKit;
using Foundation;
#endregion

namespace Mobile;

/// <summary>
/// Aim: iOS implementation for reading contact birthdays.
/// </summary>
public partial class ContactBirthdayService
{
	/// <summary>
	/// Aim: Gets all contacts with birthdays from iOS contacts.
	/// Return: List of Person objects with DisplayName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync()
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
					if (contact.Birthday == null)
						continue;

					string? contactId = contact.Identifier;
					if (string.IsNullOrWhiteSpace(contactId))
						continue;

					string displayName = $"{contact.GivenName} {contact.FamilyName}".Trim();
					if (string.IsNullOrWhiteSpace(displayName))
						continue;

					var birthdayComponents = contact.Birthday;
					int day = (int)birthdayComponents.Day;
					int month = (int)birthdayComponents.Month;
					int year = birthdayComponents.Year > 0 ? (int)birthdayComponents.Year : 0;

					if (day <= 0 || day > 31 || month <= 0 || month > 12)
						continue;

					var person = new Person
					{
						DisplayName = displayName,
						Birthday = new Birthday { Day = day, Month = month, Year = year },
						ContactId = contactId,
						Source = PersonSource.Contact
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

	/// <summary>
	/// Aim: Gets all birthdays from the iOS birthday calendar.
	/// Return: List of Person objects with DisplayName and Birthday
	/// </summary>
	public partial Task<List<Person>> GetBirthdayCalendarEventsAsync()
	{
		var results = new List<Person>();

		try
		{
			var eventStore = new EKEventStore();

			// Find birthday calendar
			var allCalendars = eventStore.GetCalendars(EKEntityType.Event);
			var birthdayCalendar = allCalendars?.FirstOrDefault(c =>
				c.Type == EKCalendarType.Birthday ||
				(c.Title?.Contains("birthday", StringComparison.OrdinalIgnoreCase) ?? false) ||
				(c.Title?.Contains("geburtstag", StringComparison.OrdinalIgnoreCase) ?? false));

			if (birthdayCalendar == null)
				return Task.FromResult(results);

			// Get events from the birthday calendar for a range
			var startDate = (NSDate)DateTime.Today.AddYears(-1);
			var endDate = (NSDate)DateTime.Today.AddYears(2);
			var predicate = eventStore.PredicateForEvents(startDate, endDate, new[] { birthdayCalendar });
			var events = eventStore.EventsMatching(predicate);

			if (events == null)
				return Task.FromResult(results);

			foreach (var evt in events)
			{
				string? title = evt.Title;
				if (string.IsNullOrWhiteSpace(title))
					continue;

				string displayName = ExtractNameFromBirthdayTitle(title);
				if (string.IsNullOrWhiteSpace(displayName))
					continue;

				var birthdayDate = (DateTime)evt.StartDate;

				var person = new Person
				{
					DisplayName = displayName,
					Birthday = BirthdayHelper.ConvertFromDateTimeToBirthday(birthdayDate),
					Source = PersonSource.BirthdayCalendar
				};

				results.Add(person);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading iOS birthday calendar: {ex.Message}");
		}

		return Task.FromResult(results);
	}

	private static string ExtractNameFromBirthdayTitle(string title)
	{
		// Common patterns: "Name's Birthday", "Birthday of Name", "Geburtstag von Name"
		string[] suffixes = ["'s birthday", "'s Birthday", " Birthday", " birthday"];
		string[] prefixes = ["Birthday of ", "Geburtstag von "];

		foreach (var suffix in suffixes)
		{
			if (title.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
				return title[..^suffix.Length].Trim();
		}

		foreach (var prefix in prefixes)
		{
			if (title.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				return title[prefix.Length..].Trim();
		}

		return title;
	}
}

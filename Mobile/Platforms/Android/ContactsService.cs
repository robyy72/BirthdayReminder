#region Usings
using Android.Content;
using Android.Database;
using Android.Provider;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Android implementation for reading contacts.
/// </summary>
public partial class ContactsService
{
	/// <summary>
	/// Aim: Gets contacts from Android contacts.
	/// Params: onlyWithBirthday - If true, only returns contacts that have a birthday set
	/// Return: List of Person objects with FirstName, LastName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsAsync(bool onlyWithBirthday)
	{
		var results = new List<Person>();

		try
		{
			var context = Platform.CurrentActivity ?? Android.App.Application.Context;
			var contentResolver = context.ContentResolver;
			if (contentResolver == null)
				return Task.FromResult(results);

			var uri = ContactsContract.Data.ContentUri;
			if (uri == null)
				return Task.FromResult(results);

			// Get all contacts with birthdays
			var contactBirthdays = GetContactBirthdays(contentResolver, uri);

			if (onlyWithBirthday)
			{
				// Only return contacts that have birthdays
				foreach (var kvp in contactBirthdays)
				{
					string contactId = kvp.Key;
					string dateString = kvp.Value;

					if (!TryParseBirthday(dateString, out Birthday? birthday) || birthday == null)
						continue;

					var (firstName, lastName) = GetContactName(contentResolver, contactId);

					if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
						continue;

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
			else
			{
				// Return ALL contacts, with or without birthday
				var allContactIds = GetAllContactIds(contentResolver);

				foreach (string contactId in allContactIds)
				{
					var (firstName, lastName) = GetContactName(contentResolver, contactId);

					if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
						continue;

					Birthday? birthday = null;
					if (contactBirthdays.TryGetValue(contactId, out string? dateString))
					{
						if (TryParseBirthday(dateString, out Birthday? parsed))
						{
							birthday = parsed;
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
			System.Diagnostics.Debug.WriteLine($"Error reading Android contacts: {ex.Message}");
		}

		return Task.FromResult(results);
	}

	/// <summary>
	/// Aim: Gets all contact IDs from the device.
	/// Params: contentResolver - Android content resolver
	/// Return: HashSet of contact IDs
	/// </summary>
	private static HashSet<string> GetAllContactIds(ContentResolver contentResolver)
	{
		var contactIds = new HashSet<string>();

		var contactsUri = ContactsContract.Contacts.ContentUri;
		if (contactsUri == null)
			return contactIds;

		string[] projection = [ContactsContract.Contacts.InterfaceConsts.Id];

		using var cursor = contentResolver.Query(contactsUri, projection, null, null, null);
		if (cursor == null)
			return contactIds;

		int idIndex = cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id);

		while (cursor.MoveToNext())
		{
			string? id = cursor.GetString(idIndex);
			if (!string.IsNullOrWhiteSpace(id))
			{
				contactIds.Add(id);
			}
		}

		return contactIds;
	}

	/// <summary>
	/// Aim: Gets all birthdays from contacts.
	/// Params: contentResolver - Android content resolver, uri - Data content URI
	/// Return: Dictionary mapping contact ID to birthday string
	/// </summary>
	private static Dictionary<string, string> GetContactBirthdays(ContentResolver contentResolver, Android.Net.Uri uri)
	{
		var contactBirthdays = new Dictionary<string, string>();

		string[] birthdayProjection =
		[
			ContactsContract.Data.InterfaceConsts.ContactId,
			ContactsContract.CommonDataKinds.Event.StartDate
		];

		string birthdaySelection = $"{ContactsContract.Data.InterfaceConsts.Mimetype} = ? AND {ContactsContract.CommonDataKinds.Event.InterfaceConsts.Type} = ?";
		string[] birthdaySelectionArgs = [ContactsContract.CommonDataKinds.Event.ContentItemType, ((int)EventDataKind.Birthday).ToString()];

		using var cursor = contentResolver.Query(uri, birthdayProjection, birthdaySelection, birthdaySelectionArgs, null);
		if (cursor == null)
			return contactBirthdays;

		int contactIdIndex = cursor.GetColumnIndex(ContactsContract.Data.InterfaceConsts.ContactId);
		int startDateIndex = cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Event.StartDate);

		while (cursor.MoveToNext())
		{
			string? contactId = cursor.GetString(contactIdIndex);
			string? dateString = cursor.GetString(startDateIndex);

			if (!string.IsNullOrWhiteSpace(contactId) && !string.IsNullOrWhiteSpace(dateString))
				contactBirthdays[contactId] = dateString;
		}

		return contactBirthdays;
	}

	/// <summary>
	/// Aim: Gets first and last name for a contact from StructuredName.
	/// Params: contentResolver - Android content resolver, contactId - Contact ID
	/// Return: Tuple with first name and last name
	/// </summary>
	private static (string FirstName, string LastName) GetContactName(ContentResolver contentResolver, string contactId)
	{
		var uri = ContactsContract.Data.ContentUri;
		if (uri == null)
			return (string.Empty, string.Empty);

		string[] projection =
		[
			ContactsContract.CommonDataKinds.StructuredName.GivenName,
			ContactsContract.CommonDataKinds.StructuredName.FamilyName
		];

		string selection = $"{ContactsContract.Data.InterfaceConsts.ContactId} = ? AND {ContactsContract.Data.InterfaceConsts.Mimetype} = ?";
		string[] selectionArgs = [contactId, ContactsContract.CommonDataKinds.StructuredName.ContentItemType];

		using var cursor = contentResolver.Query(uri, projection, selection, selectionArgs, null);
		if (cursor == null || !cursor.MoveToFirst())
			return (string.Empty, string.Empty);

		int givenNameIndex = cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.GivenName);
		int familyNameIndex = cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.FamilyName);

		string firstName = cursor.GetString(givenNameIndex) ?? string.Empty;
		string lastName = cursor.GetString(familyNameIndex) ?? string.Empty;

		return (firstName, lastName);
	}

	private static bool TryParseBirthday(string dateString, out Birthday? birthday)
	{
		birthday = null;

		// Android stores dates in various formats: yyyy-MM-dd, --MM-dd (no year), etc.
		if (dateString.StartsWith("--"))
		{
			// No year format: --MM-dd
			if (dateString.Length >= 7 &&
				int.TryParse(dateString.Substring(2, 2), out int month) &&
				int.TryParse(dateString.Substring(5, 2), out int day) &&
				month >= 1 && month <= 12 && day >= 1 && day <= 31)
			{
				birthday = new Birthday { Day = day, Month = month, Year = 0 };
				return true;
			}
		}
		else if (DateTime.TryParse(dateString, out DateTime parsed))
		{
			birthday = new Birthday { Day = parsed.Day, Month = parsed.Month, Year = parsed.Year };
			return true;
		}

		return false;
	}
}

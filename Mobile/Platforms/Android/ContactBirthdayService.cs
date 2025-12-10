#region Usings
using Android.Content;
using Android.Database;
using Android.Provider;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Android implementation for reading contact birthdays.
/// </summary>
public partial class ContactBirthdayService
{
	/// <summary>
	/// Aim: Gets all contacts with birthdays from Android contacts.
	/// Return: List of Person objects with DisplayName, Birthday and ContactId
	/// </summary>
	public partial Task<List<Person>> GetContactsWithBirthdaysAsync()
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

			string[] projection =
			[
				ContactsContract.Data.InterfaceConsts.ContactId,
				ContactsContract.CommonDataKinds.StructuredName.DisplayName,
				ContactsContract.CommonDataKinds.Event.StartDate,
				ContactsContract.CommonDataKinds.Event.InterfaceConsts.Type
			];

			string selection = $"{ContactsContract.Data.InterfaceConsts.Mimetype} = ? AND {ContactsContract.CommonDataKinds.Event.InterfaceConsts.Type} = ?";
			string[] selectionArgs = [ContactsContract.CommonDataKinds.Event.ContentItemType, ((int)EventDataKind.Birthday).ToString()];

			using var cursor = contentResolver.Query(uri, projection, selection, selectionArgs, null);
			if (cursor == null)
				return Task.FromResult(results);

			int contactIdIndex = cursor.GetColumnIndex(ContactsContract.Data.InterfaceConsts.ContactId);
			int displayNameIndex = cursor.GetColumnIndex(ContactsContract.CommonDataKinds.StructuredName.DisplayName);
			int startDateIndex = cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Event.StartDate);

			while (cursor.MoveToNext())
			{
				string? contactId = cursor.GetString(contactIdIndex);
				string? displayName = cursor.GetString(displayNameIndex);
				string? dateString = cursor.GetString(startDateIndex);

				if (string.IsNullOrWhiteSpace(contactId))
					continue;

				if (string.IsNullOrWhiteSpace(displayName))
					continue;

				if (string.IsNullOrWhiteSpace(dateString))
					continue;

				if (!TryParseBirthdayDate(dateString, out DateTime birthdayDate))
					continue;

				var person = new Person
				{
					DisplayName = displayName,
					Birthday = BirthdayHelper.ConvertFromDateTimeToBirthday(birthdayDate),
					ContactId = contactId,
					Source = PersonSource.Contact
				};

				results.Add(person);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading Android contacts: {ex.Message}");
		}

		return Task.FromResult(results);
	}

	/// <summary>
	/// Aim: Gets all birthdays from the Android birthday calendar.
	/// Return: List of Person objects with DisplayName and Birthday
	/// </summary>
	public partial Task<List<Person>> GetBirthdayCalendarEventsAsync()
	{
		var results = new List<Person>();

		try
		{
			var context = Platform.CurrentActivity ?? Android.App.Application.Context;
			var contentResolver = context.ContentResolver;
			if (contentResolver == null)
				return Task.FromResult(results);

			// Find birthday calendar
			long? birthdayCalendarId = FindBirthdayCalendarId(contentResolver);
			if (birthdayCalendarId == null)
				return Task.FromResult(results);

			var uri = CalendarContract.Events.ContentUri;
			if (uri == null)
				return Task.FromResult(results);

			string[] projection =
			[
				CalendarContract.Events.InterfaceConsts.Title,
				CalendarContract.Events.InterfaceConsts.Dtstart
			];

			string selection = $"{CalendarContract.Events.InterfaceConsts.CalendarId} = ?";
			string[] selectionArgs = [birthdayCalendarId.Value.ToString()];

			using var cursor = contentResolver.Query(uri, projection, selection, selectionArgs, null);
			if (cursor == null)
				return Task.FromResult(results);

			int titleIndex = cursor.GetColumnIndex(CalendarContract.Events.InterfaceConsts.Title);
			int dtstartIndex = cursor.GetColumnIndex(CalendarContract.Events.InterfaceConsts.Dtstart);

			while (cursor.MoveToNext())
			{
				string? title = cursor.GetString(titleIndex);
				long dtstart = cursor.GetLong(dtstartIndex);

				if (string.IsNullOrWhiteSpace(title))
					continue;

				// Extract name from title (usually "Name's Birthday" or "Geburtstag von Name")
				string displayName = ExtractNameFromBirthdayTitle(title);
				if (string.IsNullOrWhiteSpace(displayName))
					continue;

				var birthdayDate = DateTimeOffset.FromUnixTimeMilliseconds(dtstart).DateTime;

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
			System.Diagnostics.Debug.WriteLine($"Error reading Android birthday calendar: {ex.Message}");
		}

		return Task.FromResult(results);
	}

	private static long? FindBirthdayCalendarId(ContentResolver contentResolver)
	{
		var uri = CalendarContract.Calendars.ContentUri;
		if (uri == null)
			return null;

		string[] projection =
		[
			CalendarContract.Calendars.InterfaceConsts.Id,
			CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
			CalendarContract.Calendars.InterfaceConsts.AccountType
		];

		using var cursor = contentResolver.Query(uri, projection, null, null, null);
		if (cursor == null)
			return null;

		int idIndex = cursor.GetColumnIndex(CalendarContract.Calendars.InterfaceConsts.Id);
		int displayNameIndex = cursor.GetColumnIndex(CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName);
		int accountTypeIndex = cursor.GetColumnIndex(CalendarContract.Calendars.InterfaceConsts.AccountType);

		while (cursor.MoveToNext())
		{
			string? displayName = cursor.GetString(displayNameIndex);
			string? accountType = cursor.GetString(accountTypeIndex);

			// Look for birthday calendars
			bool isBirthdayCalendar =
				(displayName?.Contains("birthday", StringComparison.OrdinalIgnoreCase) ?? false) ||
				(displayName?.Contains("geburtstag", StringComparison.OrdinalIgnoreCase) ?? false) ||
				(accountType?.Contains("birthday", StringComparison.OrdinalIgnoreCase) ?? false);

			if (isBirthdayCalendar)
				return cursor.GetLong(idIndex);
		}

		return null;
	}

	private static bool TryParseBirthdayDate(string dateString, out DateTime birthday)
	{
		birthday = default;

		// Android stores dates in various formats: yyyy-MM-dd, --MM-dd (no year), etc.
		if (dateString.StartsWith("--"))
		{
			// No year format: --MM-dd
			if (dateString.Length >= 7 &&
				int.TryParse(dateString.Substring(2, 2), out int month) &&
				int.TryParse(dateString.Substring(5, 2), out int day))
			{
				birthday = new DateTime(1900, month, day);
				return true;
			}
		}
		else if (DateTime.TryParse(dateString, out DateTime parsed))
		{
			birthday = parsed;
			return true;
		}

		return false;
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

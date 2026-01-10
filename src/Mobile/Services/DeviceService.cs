#region Usings
using Microsoft.Maui.ApplicationModel;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Provides device-related functionality like theme detection and permission handling
/// </summary>
public static class DeviceService
{
	#region Timezone Mapping
	/// <summary>
	/// Maps timezone IDs (Windows and IANA) to resource key suffixes
	/// </summary>
	private static readonly Dictionary<string, string> TimezoneMapping = new(StringComparer.OrdinalIgnoreCase)
	{
		// UTC
		{ "UTC", "UTC" },
		{ "Etc/UTC", "UTC" },
		{ "Etc/GMT", "GMT" },

		// Central European Time
		{ "Central European Standard Time", "CET" },
		{ "W. Europe Standard Time", "CET" },
		{ "Romance Standard Time", "CET" },
		{ "Central Europe Standard Time", "CET" },
		{ "Europe/Berlin", "CET" },
		{ "Europe/Paris", "CET" },
		{ "Europe/Rome", "CET" },
		{ "Europe/Amsterdam", "CET" },
		{ "Europe/Brussels", "CET" },
		{ "Europe/Vienna", "CET" },
		{ "Europe/Zurich", "CET" },
		{ "Europe/Madrid", "CET" },
		{ "Europe/Warsaw", "CET" },
		{ "Europe/Prague", "CET" },
		{ "Europe/Budapest", "CET" },
		{ "Europe/Stockholm", "CET" },
		{ "Europe/Oslo", "CET" },
		{ "Europe/Copenhagen", "CET" },

		// Western European Time
		{ "GMT Standard Time", "WET" },
		{ "Greenwich Standard Time", "GMT" },
		{ "Europe/London", "WET" },
		{ "Europe/Dublin", "WET" },
		{ "Europe/Lisbon", "WET" },
		{ "Atlantic/Reykjavik", "GMT" },

		// Eastern European Time
		{ "E. Europe Standard Time", "EET" },
		{ "GTB Standard Time", "EET" },
		{ "FLE Standard Time", "EET" },
		{ "Europe/Athens", "EET" },
		{ "Europe/Helsinki", "EET" },
		{ "Europe/Bucharest", "EET" },
		{ "Europe/Sofia", "EET" },
		{ "Europe/Kiev", "EET" },
		{ "Europe/Kyiv", "EET" },
		{ "Europe/Tallinn", "EET" },
		{ "Europe/Vilnius", "EET" },
		{ "Europe/Riga", "EET" },

		// US Eastern
		{ "Eastern Standard Time", "EST" },
		{ "America/New_York", "EST" },
		{ "America/Detroit", "EST" },
		{ "America/Toronto", "EST" },

		// US Central
		{ "Central Standard Time", "CST" },
		{ "America/Chicago", "CST" },
		{ "America/Mexico_City", "CST" },

		// US Mountain
		{ "Mountain Standard Time", "MST" },
		{ "US Mountain Standard Time", "MST" },
		{ "America/Denver", "MST" },
		{ "America/Phoenix", "MST" },

		// US Pacific
		{ "Pacific Standard Time", "PST" },
		{ "America/Los_Angeles", "PST" },
		{ "America/Vancouver", "PST" },

		// Alaska
		{ "Alaskan Standard Time", "AKST" },
		{ "America/Anchorage", "AKST" },

		// Hawaii
		{ "Hawaiian Standard Time", "HST" },
		{ "Pacific/Honolulu", "HST" },

		// Japan
		{ "Tokyo Standard Time", "JST" },
		{ "Asia/Tokyo", "JST" },

		// China
		{ "China Standard Time", "CST_China" },
		{ "Asia/Shanghai", "CST_China" },
		{ "Asia/Hong_Kong", "HKT" },

		// India
		{ "India Standard Time", "IST" },
		{ "Asia/Kolkata", "IST" },
		{ "Asia/Calcutta", "IST" },

		// Australia
		{ "AUS Eastern Standard Time", "AEST" },
		{ "E. Australia Standard Time", "AEST" },
		{ "Australia/Sydney", "AEST" },
		{ "Australia/Melbourne", "AEST" },
		{ "W. Australia Standard Time", "AWST" },
		{ "Australia/Perth", "AWST" },

		// New Zealand
		{ "New Zealand Standard Time", "NZST" },
		{ "Pacific/Auckland", "NZST" },

		// Brazil
		{ "E. South America Standard Time", "BRT" },
		{ "America/Sao_Paulo", "BRT" },

		// Argentina
		{ "Argentina Standard Time", "ART" },
		{ "America/Buenos_Aires", "ART" },
		{ "America/Argentina/Buenos_Aires", "ART" },

		// Russia
		{ "Russian Standard Time", "MSK" },
		{ "Europe/Moscow", "MSK" },

		// Gulf
		{ "Arabian Standard Time", "GST" },
		{ "Asia/Dubai", "GST" },

		// Singapore
		{ "Singapore Standard Time", "SGT" },
		{ "Asia/Singapore", "SGT" },

		// Korea
		{ "Korea Standard Time", "KST" },
		{ "Asia/Seoul", "KST" },

		// Thailand/Vietnam
		{ "SE Asia Standard Time", "ICT" },
		{ "Asia/Bangkok", "ICT" },
		{ "Asia/Ho_Chi_Minh", "ICT" },

		// Indonesia
		{ "Asia/Jakarta", "WIB" },

		// Pakistan
		{ "Pakistan Standard Time", "PKT" },
		{ "Asia/Karachi", "PKT" },

		// Saudi Arabia
		{ "Arab Standard Time", "AST" },
		{ "Asia/Riyadh", "AST" },

		// South Africa
		{ "South Africa Standard Time", "SAST" },
		{ "Africa/Johannesburg", "SAST" },

		// West Africa
		{ "W. Central Africa Standard Time", "WAT" },
		{ "Africa/Lagos", "WAT" },

		// East Africa
		{ "E. Africa Standard Time", "EAT" },
		{ "Africa/Nairobi", "EAT" },

		// Turkey
		{ "Turkey Standard Time", "TRT" },
		{ "Europe/Istanbul", "TRT" },
	};
	#endregion

	#region Theme
	/// <summary>
	/// Aim: Gets the current system theme
	/// Return: AppTheme (Light, Dark, or Unspecified)
	/// </summary>
	public static AppTheme GetSystemTheme()
	{
		AppTheme theme = Application.Current?.RequestedTheme ?? AppTheme.Light;
		return theme;
	}

	/// <summary>
	/// Aim: Checks if the system is using dark theme
	/// Return: True if dark theme, false otherwise
	/// </summary>
	public static bool IsDarkTheme()
	{
		bool isDark = ResourceHelper.IsDarkTheme;
		return isDark;
	}

	/// <summary>
	/// Aim: Applies the specified theme to the application
	/// Params: theme - The theme string (Light, Dark, or System)
	/// </summary>
	public static void ApplyTheme(string theme)
	{
		switch (theme)
		{
			case "Light":
				Application.Current!.UserAppTheme = AppTheme.Light;
				break;
			case "Dark":
				Application.Current!.UserAppTheme = AppTheme.Dark;
				break;
			default:
				Application.Current!.UserAppTheme = AppTheme.Unspecified;
				break;
		}
	}
	#endregion

	#region Permissions
	/// <summary>
	/// Aim: Checks if contacts read permission is granted
	/// Return: True if granted (full or limited), false otherwise
	/// </summary>
	public static async Task<bool> CheckContactsReadPermissionAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
		bool isGranted = status == PermissionStatus.Granted || status == PermissionStatus.Limited;
		return isGranted;
	}

	/// <summary>
	/// Aim: Gets the contacts access choice (iOS only: full vs limited access)
	/// Return: ContactsAccessChoice enum value
	/// </summary>
	public static async Task<ContactsAccessChoice> GetContactsAccessChoiceAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();

		if (status == PermissionStatus.Limited)
			return ContactsAccessChoice.OnlySomeContacts;

		if (status == PermissionStatus.Granted)
			return ContactsAccessChoice.AllContacts;

		return ContactsAccessChoice.NotSet;
	}

	/// <summary>
	/// Aim: Requests contacts read permission
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> RequestContactsReadPermissionAsync()
	{
		try
		{
			PermissionStatus status = await Permissions.RequestAsync<Permissions.ContactsRead>();
			bool isGranted = status == PermissionStatus.Granted;
			return isGranted;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Contacts read permission error: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Aim: Checks if contacts write permission is granted
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> CheckContactsWritePermissionAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.ContactsWrite>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
	}

	/// <summary>
	/// Aim: Requests contacts write permission
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> RequestContactsWritePermissionAsync()
	{
		try
		{
			PermissionStatus status = await Permissions.RequestAsync<Permissions.ContactsWrite>();
			bool isGranted = status == PermissionStatus.Granted;
			return isGranted;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Contacts write permission error: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Aim: Checks if calendar read permission is granted
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> CheckCalendarReadPermissionAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
	}

	/// <summary>
	/// Aim: Requests calendar read permission
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> RequestCalendarReadPermissionAsync()
	{
		try
		{
			PermissionStatus status = await Permissions.RequestAsync<Permissions.CalendarRead>();
			bool isGranted = status == PermissionStatus.Granted;
			return isGranted;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Calendar read permission error: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Aim: Checks if calendar write permission is granted
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> CheckCalendarWritePermissionAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
	}

	/// <summary>
	/// Aim: Requests calendar write permission
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> RequestCalendarWritePermissionAsync()
	{
		try
		{
			PermissionStatus status = await Permissions.RequestAsync<Permissions.CalendarWrite>();
			bool isGranted = status == PermissionStatus.Granted;
			return isGranted;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Calendar write permission error: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// Aim: Shows a permission denied alert and optionally opens settings
	/// Params: title - Alert title, message - Alert message
	/// </summary>
	public static async Task ShowPermissionDeniedAlertAsync(string title, string message)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			var page = Application.Current.Windows[0].Page;
			if (page != null)
			{
				bool openSettings = await page.DisplayAlert(
					title,
					message,
					MobileLanguages.Resources.General_Button_OK,
					MobileLanguages.Resources.General_Button_Cancel);

				if (openSettings)
				{
					AppInfo.ShowSettingsUI();
				}
			}
		}
	}
	#endregion

	#region TimeZone
	/// <summary>
	/// Aim: Gets the device's current time zone ID
	/// Return: Time zone ID string (e.g., "Europe/Berlin")
	/// </summary>
	public static string GetTimeZoneId()
	{
		string timeZoneId = TimeZoneInfo.Local.Id;
		return timeZoneId;
	}

	/// <summary>
	/// Aim: Gets a timezone abbreviation from the account's timezone ID
	/// Return: Timezone abbreviation (e.g., "CET", "PST", "EST")
	/// </summary>
	public static string GetTimeZoneAbbreviation()
	{
		try
		{
			string timeZoneId = App.Account.TimeZoneId;
			if (string.IsNullOrEmpty(timeZoneId))
			{
				timeZoneId = TimeZoneInfo.Local.Id;
			}

			var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			bool isDaylightSaving = timeZone.IsDaylightSavingTime(DateTime.Now);
			string name = isDaylightSaving ? timeZone.DaylightName : timeZone.StandardName;

			// Extract initials from timezone name (e.g., "Central European Time" -> "CET")
			var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (words.Length > 1)
			{
				string abbreviation = string.Concat(words.Select(w => char.ToUpper(w[0])));
				return abbreviation;
			}

			// Fallback: return first 3-4 chars of the ID
			string result = timeZoneId.Length > 4 ? timeZoneId[..4] : timeZoneId;
			return result;
		}
		catch
		{
			return "UTC";
		}
	}

	/// <summary>
	/// Aim: Gets timezone information as separate parts (abbreviation, full name, offset)
	/// Return: Tuple with abbreviation, full name, and offset string
	/// </summary>
	public static (string Abbreviation, string FullName, string Offset) GetTimeZoneInfo()
	{
		try
		{
			string timeZoneId = App.Account.TimeZoneId;
			if (string.IsNullOrEmpty(timeZoneId))
			{
				timeZoneId = TimeZoneInfo.Local.Id;
			}

			var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			bool isDaylightSaving = timeZone.IsDaylightSavingTime(DateTime.Now);

			// Get abbreviation and full name from resources
			string abbreviation = GetTimeZoneAbbreviation();
			string fullName = GetLocalizedTimezoneName(timeZoneId, isDaylightSaving);

			// Format offset
			TimeSpan offset = timeZone.GetUtcOffset(DateTime.Now);
			string offsetSign = offset >= TimeSpan.Zero ? "+" : "-";
			string offsetStr = $"{offsetSign}{Math.Abs(offset.Hours):00}:{Math.Abs(offset.Minutes):00}";

			return (abbreviation, fullName, offsetStr);
		}
		catch
		{
			string fallbackName = MobileLanguages.Resources.Timezone_UTC;
			return ("UTC", fallbackName, "+00:00");
		}
	}

	/// <summary>
	/// Aim: Gets the localized timezone name from resources
	/// Params: timeZoneId - The timezone ID, isDaylightSaving - Whether daylight saving is active
	/// Return: Localized timezone name
	/// </summary>
	private static string GetLocalizedTimezoneName(string timeZoneId, bool isDaylightSaving)
	{
		// Try to find the timezone in our mapping
		if (TimezoneMapping.TryGetValue(timeZoneId, out string? resourceKey))
		{
			// Adjust key for daylight saving time
			string adjustedKey = GetDaylightAdjustedKey(resourceKey, isDaylightSaving);
			string? localizedName = GetTimezoneResourceByKey(adjustedKey);
			if (!string.IsNullOrEmpty(localizedName))
			{
				return localizedName;
			}

			// Fall back to standard key if daylight key not found
			localizedName = GetTimezoneResourceByKey(resourceKey);
			if (!string.IsNullOrEmpty(localizedName))
			{
				return localizedName;
			}
		}

		// Fall back to unknown timezone
		string unknownName = MobileLanguages.Resources.Timezone_Unknown;
		return unknownName;
	}

	/// <summary>
	/// Aim: Adjusts the resource key for daylight saving time
	/// </summary>
	private static string GetDaylightAdjustedKey(string baseKey, bool isDaylightSaving)
	{
		if (!isDaylightSaving)
		{
			return baseKey;
		}

		// Map standard keys to their daylight equivalents
		var daylightMapping = new Dictionary<string, string>
		{
			{ "CET", "CEST" },
			{ "WET", "WEST" },
			{ "EET", "EEST" },
			{ "GMT", "BST" },
			{ "EST", "EDT" },
			{ "CST", "CDT" },
			{ "MST", "MDT" },
			{ "PST", "PDT" },
			{ "AKST", "AKDT" },
			{ "AEST", "AEDT" },
			{ "NZST", "NZDT" },
		};

		if (daylightMapping.TryGetValue(baseKey, out string? daylightKey))
		{
			return daylightKey;
		}

		return baseKey;
	}

	/// <summary>
	/// Aim: Gets a timezone resource string by its key suffix
	/// </summary>
	private static string? GetTimezoneResourceByKey(string keySuffix)
	{
		string fullKey = $"Timezone_{keySuffix}";
		string? result = MobileLanguages.Resources.ResourceManager.GetString(fullKey);
		return result;
	}
	#endregion

	#region Device Info
	/// <summary>
	/// Aim: Gets the device platform (Android, iOS, etc.)
	/// Return: DevicePlatform
	/// </summary>
	public static DevicePlatform GetPlatform()
	{
		DevicePlatform platform = DeviceInfo.Platform;
		return platform;
	}

	/// <summary>
	/// Aim: Checks if running on Android
	/// Return: True if Android, false otherwise
	/// </summary>
	public static bool IsAndroid()
	{
		bool isAndroid = DeviceInfo.Platform == DevicePlatform.Android;
		return isAndroid;
	}

	/// <summary>
	/// Aim: Checks if running on iOS
	/// Return: True if iOS, false otherwise
	/// </summary>
	public static bool IsIOS()
	{
		bool isIOS = DeviceInfo.Platform == DevicePlatform.iOS;
		return isIOS;
	}
	#endregion
}

#region Usings
using MobileLanguages;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Provides curated timezone list with dynamic offsets.
/// </summary>
public static class TimezoneService
{
	#region Private Fields
	private static List<TimezoneEntry>? _cachedEntries;
	private static DateTime _cacheTime = DateTime.MinValue;
	#endregion

	#region Timezone Definitions
	/// <summary>
	/// Curated list of important timezones with IANA IDs and resource keys.
	/// Sorted by approximate base offset.
	/// </summary>
	private static readonly (string Id, string ResourceKey)[] TimezoneDefinitions =
	[
		// UTC-11 to UTC-8
		("Pacific/Midway", "Tz_Pacific_Midway"),
		("Pacific/Honolulu", "Tz_Pacific_Honolulu"),
		("America/Anchorage", "Tz_America_Anchorage"),
		("America/Los_Angeles", "Tz_America_LosAngeles"),

		// UTC-7 to UTC-5
		("America/Denver", "Tz_America_Denver"),
		("America/Chicago", "Tz_America_Chicago"),
		("America/New_York", "Tz_America_NewYork"),

		// UTC-4 to UTC-3
		("America/Halifax", "Tz_America_Halifax"),
		("America/Sao_Paulo", "Tz_America_SaoPaulo"),
		("America/Buenos_Aires", "Tz_America_BuenosAires"),

		// UTC-1 to UTC
		("Atlantic/Azores", "Tz_Atlantic_Azores"),
		("Europe/London", "Tz_Europe_London"),

		// UTC+1
		("Europe/Berlin", "Tz_Europe_Berlin"),
		("Europe/Paris", "Tz_Europe_Paris"),
		("Europe/Rome", "Tz_Europe_Rome"),
		("Europe/Madrid", "Tz_Europe_Madrid"),
		("Europe/Amsterdam", "Tz_Europe_Amsterdam"),
		("Europe/Vienna", "Tz_Europe_Vienna"),
		("Europe/Zurich", "Tz_Europe_Zurich"),
		("Europe/Warsaw", "Tz_Europe_Warsaw"),

		// UTC+2 to UTC+3
		("Europe/Athens", "Tz_Europe_Athens"),
		("Europe/Helsinki", "Tz_Europe_Helsinki"),
		("Europe/Istanbul", "Tz_Europe_Istanbul"),
		("Europe/Moscow", "Tz_Europe_Moscow"),
		("Asia/Dubai", "Tz_Asia_Dubai"),

		// UTC+5 to UTC+6
		("Asia/Karachi", "Tz_Asia_Karachi"),
		("Asia/Kolkata", "Tz_Asia_Kolkata"),
		("Asia/Dhaka", "Tz_Asia_Dhaka"),

		// UTC+7 to UTC+9
		("Asia/Bangkok", "Tz_Asia_Bangkok"),
		("Asia/Singapore", "Tz_Asia_Singapore"),
		("Asia/Hong_Kong", "Tz_Asia_HongKong"),
		("Asia/Shanghai", "Tz_Asia_Shanghai"),
		("Asia/Tokyo", "Tz_Asia_Tokyo"),
		("Asia/Seoul", "Tz_Asia_Seoul"),

		// UTC+10 to UTC+12
		("Australia/Sydney", "Tz_Australia_Sydney"),
		("Pacific/Auckland", "Tz_Pacific_Auckland"),
	];
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Loads curated timezone list with current offsets.
	/// Params: includeDeviceTimezone - Whether to add device timezone at top.
	/// Return (List&lt;TimezoneEntry&gt;): Sorted list of timezone entries.
	/// </summary>
	public static List<TimezoneEntry> LoadTimezones(bool includeDeviceTimezone = true)
	{
		// Return cached if less than 1 hour old (DST doesn't change that often)
		if (_cachedEntries != null && DateTime.Now - _cacheTime < TimeSpan.FromHours(1))
		{
			return _cachedEntries;
		}

		var entries = new List<TimezoneEntry>();

		// Add device timezone first if requested
		if (includeDeviceTimezone)
		{
			var deviceTz = TimeZoneInfo.Local;
			var deviceOffset = deviceTz.GetUtcOffset(DateTime.Now);
			var deviceLabel = GetLocalizedName("Tz_Device");
			var deviceEntry = new TimezoneEntry
			{
				Id = deviceTz.Id,
				DisplayName = $"{deviceLabel} [{FormatOffset(deviceOffset)}]",
				Offset = deviceOffset,
				IsDeviceTimezone = true
			};
			entries.Add(deviceEntry);
		}

		// Add curated timezones
		foreach (var (id, resourceKey) in TimezoneDefinitions)
		{
			try
			{
				var tz = TimeZoneInfo.FindSystemTimeZoneById(id);
				var offset = tz.GetUtcOffset(DateTime.Now);
				var cityName = GetLocalizedName(resourceKey);

				var entry = new TimezoneEntry
				{
					Id = id,
					DisplayName = $"{cityName} [{FormatOffset(offset)}]",
					Offset = offset,
					IsDeviceTimezone = false
				};
				entries.Add(entry);
			}
			catch (TimeZoneNotFoundException)
			{
				System.Diagnostics.Debug.WriteLine($"Timezone not found: {id}");
			}
		}

		// Sort by offset (device timezone stays first)
		var sorted = entries
			.OrderBy(e => !e.IsDeviceTimezone)
			.ThenBy(e => e.Offset)
			.ToList();

		_cachedEntries = sorted;
		_cacheTime = DateTime.Now;

		return sorted;
	}

	/// <summary>
	/// Aim: Gets display names for picker.
	/// Return (List&lt;string&gt;): List of formatted display names.
	/// </summary>
	public static List<string> GetDisplayNames()
	{
		var entries = LoadTimezones();
		var names = entries.Select(e => e.DisplayName).ToList();
		return names;
	}

	/// <summary>
	/// Aim: Finds timezone entry by ID.
	/// Params: id - Timezone ID to find.
	/// Return (TimezoneEntry?): Found entry or null.
	/// </summary>
	public static TimezoneEntry? FindById(string? id)
	{
		if (string.IsNullOrEmpty(id))
			return null;

		var entries = LoadTimezones();
		var entry = entries.FirstOrDefault(e => e.Id == id);
		return entry;
	}

	/// <summary>
	/// Aim: Gets index of timezone in list.
	/// Params: id - Timezone ID to find.
	/// Return (int): Index or 0 if not found.
	/// </summary>
	public static int GetIndex(string? id)
	{
		if (string.IsNullOrEmpty(id))
			return 0;

		var entries = LoadTimezones();
		var index = entries.FindIndex(e => e.Id == id);
		return index >= 0 ? index : 0;
	}

	/// <summary>
	/// Aim: Gets timezone ID by index.
	/// Params: index - Index in the list.
	/// Return (string): Timezone ID or device timezone ID.
	/// </summary>
	public static string GetIdByIndex(int index)
	{
		var entries = LoadTimezones();
		if (index >= 0 && index < entries.Count)
		{
			var id = entries[index].Id;
			return id;
		}
		var deviceId = TimeZoneInfo.Local.Id;
		return deviceId;
	}
	#endregion

	#region Private Methods
	private static string FormatOffset(TimeSpan offset)
	{
		var sign = offset >= TimeSpan.Zero ? "+" : "-";
		var formatted = $"{sign}{Math.Abs(offset.Hours):00}:{Math.Abs(offset.Minutes):00}";
		return formatted;
	}

	private static string GetLocalizedName(string resourceKey)
	{
		var name = Resources.ResourceManager.GetString(resourceKey) ?? resourceKey;
		return name;
	}
	#endregion
}

/// <summary>
/// Aim: Represents a timezone entry for display.
/// </summary>
public class TimezoneEntry
{
	public string Id { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public TimeSpan Offset { get; set; }
	public bool IsDeviceTimezone { get; set; }
}

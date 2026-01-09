#region Usings
using System.Text.Json;
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
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Loads curated timezone list with current offsets.
	/// Return (List&lt;TimezoneEntry&gt;): List of timezone entries.
	/// </summary>
	public static List<TimezoneEntry> LoadTimezones()
	{
		if (_cachedEntries != null)
		{
			return _cachedEntries;
		}

		var jsonEntries = LoadFromJson();
		var entries = jsonEntries.Select(CreateTimezoneEntry).ToList();

		_cachedEntries = entries;
		return entries;
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
	/// Aim: Gets index of timezone in list.
	/// Params: id - Timezone ID to find.
	/// Return (int): Index or closest match if not found.
	/// </summary>
	public static int GetIndex(string? id)
	{
		if (string.IsNullOrEmpty(id))
			return FindClosestIndex(TimeZoneInfo.Local);

		var entries = LoadTimezones();
		int index = entries.FindIndex(e => e.Id == id);

		if (index < 0)
			index = FindClosestIndex(id);

		return index;
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
	private static List<TimezoneJsonEntry> LoadFromJson()
	{
		using var stream = FileSystem.OpenAppPackageFileAsync("timezones.json").Result;
		using var reader = new StreamReader(stream);
		var json = reader.ReadToEnd();
		var entries = JsonSerializer.Deserialize<List<TimezoneJsonEntry>>(json);
		return entries ?? throw new InvalidOperationException("Failed to deserialize timezones.json");
	}

	private static TimezoneEntry CreateTimezoneEntry(TimezoneJsonEntry json)
	{
		var tz = TimeZoneInfo.FindSystemTimeZoneById(json.Id);
		var offset = tz.GetUtcOffset(DateTime.Now);
		var city = GetLocalizedCity(json.Id);
		var entry = new TimezoneEntry
		{
			Id = json.Id,
			DisplayName = $"{json.Region} ({FormatOffset(offset)}h) | {city}",
			Offset = offset
		};
		return entry;
	}

	private static string GetLocalizedCity(string timezoneId)
	{
		var parts = timezoneId.Split('/');
		var region = parts[0];
		var city = parts.Length > 1 ? parts[^1].Replace("_", "") : "";
		var resourceKey = $"Tz_{region}_{city}";
		var localizedCity = Resources.ResourceManager.GetString(resourceKey);
		return localizedCity ?? city;
	}

	private static int FindClosestIndex(string timezoneId)
	{
		var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
		int index = FindClosestIndex(tz);
		return index;
	}

	private static int FindClosestIndex(TimeZoneInfo tz)
	{
		var entries = LoadTimezones();
		var deviceOffset = tz.GetUtcOffset(DateTime.Now);

		int index = entries
			.Select((e, i) => (Entry: e, Index: i))
			.OrderBy(x => Math.Abs((x.Entry.Offset - deviceOffset).TotalMinutes))
			.First().Index;

		return index;
	}

	private static string FormatOffset(TimeSpan offset)
	{
		int hours = (int)offset.TotalHours;
		int minutes = Math.Abs(offset.Minutes);
		string sign = offset.TotalHours >= 0 ? "+" : "";

		if (minutes == 0)
		{
			var formatted = $"{sign}{hours}";
			return formatted;
		}
		else
		{
			var formatted = $"{sign}{hours}:{minutes:D2}";
			return formatted;
		}
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
}

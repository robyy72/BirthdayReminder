
#region Usings
using System.Text.Json;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Helper class for storing and retrieving preferences using MAUI Preferences API.
/// </summary>
public static class PrefsHelper
{
	/// <summary>
	/// Aim: Get a string value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored string or default value.</returns>
	public static string GetValue(string key, string defaultValue = "")
	{
		return Preferences.Get(key, defaultValue);
	}

	/// <summary>
	/// Aim: Get an int value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored int or default value.</returns>
	public static int GetValue(string key, int defaultValue)
	{
		return Preferences.Get(key, defaultValue);
	}

	/// <summary>
	/// Aim: Get a bool value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored bool or default value.</returns>
	public static bool GetValue(string key, bool defaultValue)
	{
		return Preferences.Get(key, defaultValue);
	}

	/// <summary>
	/// Aim: Get a double value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored double or default value.</returns>
	public static double GetValue(string key, double defaultValue)
	{
		return Preferences.Get(key, defaultValue);
	}

	/// <summary>
	/// Aim: Get a DateTimeOffset value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored DateTimeOffset or default value.</returns>
	public static DateTimeOffset GetValue(string key, DateTimeOffset defaultValue)
	{
		var ticks = Preferences.Get(key, defaultValue.Ticks);
		var offset = TimeSpan.FromMinutes(Preferences.Get($"{key}_offset", (int)defaultValue.Offset.TotalMinutes));
		return new DateTimeOffset(ticks, offset);
	}

	/// <summary>
	/// Aim: Get a DateOnly value from preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found.</param>
	/// <returns>The stored DateOnly or default value.</returns>
	public static DateOnly GetValue(string key, DateOnly defaultValue)
	{
		var dayNumber = Preferences.Get(key, defaultValue.DayNumber);
		return DateOnly.FromDayNumber(dayNumber);
	}

	/// <summary>
	/// Aim: Get an object value from preferences (JSON serialized).
	/// </summary>
	/// <typeparam name="T">The type to deserialize to.</typeparam>
	/// <param name="key">The preference key.</param>
	/// <param name="defaultValue">Default value if key not found or deserialization fails.</param>
	/// <returns>The deserialized object or default value.</returns>
	public static T? GetValue<T>(string key, T? defaultValue = default) where T : class
	{
		var json = Preferences.Get(key, string.Empty);
		if (string.IsNullOrEmpty(json))
		{
			return defaultValue;
		}

		try
		{
			return JsonSerializer.Deserialize<T>(json) ?? defaultValue;
		}
		catch
		{
			return defaultValue;
		}
	}

	/// <summary>
	/// Aim: Set a string value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, string value)
	{
		Preferences.Set(key, value);
	}

	/// <summary>
	/// Aim: Set an int value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, int value)
	{
		Preferences.Set(key, value);
	}

	/// <summary>
	/// Aim: Set a bool value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, bool value)
	{
		Preferences.Set(key, value);
	}

	/// <summary>
	/// Aim: Set a double value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, double value)
	{
		Preferences.Set(key, value);
	}

	/// <summary>
	/// Aim: Set a DateTimeOffset value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, DateTimeOffset value)
	{
		Preferences.Set(key, value.Ticks);
		Preferences.Set($"{key}_offset", (int)value.Offset.TotalMinutes);
	}

	/// <summary>
	/// Aim: Set a DateOnly value in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The value to store.</param>
	public static void SetValue(string key, DateOnly value)
	{
		Preferences.Set(key, value.DayNumber);
	}

	/// <summary>
	/// Aim: Set an object value in preferences (JSON serialized).
	/// </summary>
	/// <typeparam name="T">The type of object to serialize.</typeparam>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The object to store.</param>
	public static void SetValue<T>(string key, T value) where T : class
	{
		var json = JsonSerializer.Serialize(value);
		Preferences.Set(key, json);
	}

	/// <summary>
	/// Aim: Check if a key exists in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <returns>True if the key exists, false otherwise.</returns>
	public static bool Exists(string key)
	{
		return Preferences.ContainsKey(key);
	}

	/// <summary>
	/// Aim: Remove a key from preferences.
	/// </summary>
	/// <param name="key">The preference key to remove.</param>
	public static void RemoveKey(string key)
	{
		Preferences.Remove(key);
		Preferences.Remove($"{key}_offset");
	}

	/// <summary>
	/// Aim: Remove all keys from preferences.
	/// </summary>
	public static void RemoveAllKeys()
	{
		Preferences.Clear();
	}
}

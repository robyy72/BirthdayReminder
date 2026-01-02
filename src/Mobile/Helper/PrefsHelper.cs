
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
	/// Aim: Get a value from preferences.
	/// </summary>
	/// <typeparam name="T">The type to retrieve.</typeparam>
	/// <param name="key">The preference key.</param>
	/// <returns>The stored value or default for type T.</returns>
	public static T? GetValue<T>(string key)
	{
		switch (typeof(T))
		{
			case Type t when t == typeof(bool):
				bool boolValue = Preferences.Get(key, false);
				return (T)(object)boolValue;

			case Type t when t == typeof(int):
				int intValue = Preferences.Get(key, 0);
				return (T)(object)intValue;

			case Type t when t == typeof(double):
				double doubleValue = Preferences.Get(key, 0.0);
				return (T)(object)doubleValue;

			case Type t when t == typeof(string):
				string stringValue = Preferences.Get(key, string.Empty);
				return (T)(object)stringValue;

			case Type t when t == typeof(DateTimeOffset):
				long ticks = Preferences.Get(key, 0L);
				if (ticks == 0L)
					return default;
				int offsetMinutes = Preferences.Get($"{key}_offset", 0);
				DateTimeOffset dtoValue = new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
				return (T)(object)dtoValue;

			default:
				string json = Preferences.Get(key, string.Empty);
				if (string.IsNullOrEmpty(json))
					return default;
				try
				{
					T? deserializedValue = JsonSerializer.Deserialize<T>(json);
					return deserializedValue;
				}
				catch
				{
					return default;
				}
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
	/// Aim: Set an object value in preferences (JSON serialized).
	/// </summary>
	/// <typeparam name="T">The type of object to serialize.</typeparam>
	/// <param name="key">The preference key.</param>
	/// <param name="value">The object to store.</param>
	public static void SetValue<T>(string key, T value) where T : class
	{
		string json = JsonSerializer.Serialize(value);
		Preferences.Set(key, json);
	}

	/// <summary>
	/// Aim: Check if a key exists in preferences.
	/// </summary>
	/// <param name="key">The preference key.</param>
	/// <returns>True if the key exists, false otherwise.</returns>
	public static bool Exists(string key)
	{
		bool exists = Preferences.ContainsKey(key);
		return exists;
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

namespace Mobile;

/// <summary>
/// Aim: Service for managing application settings.
/// </summary>
public static class SettingsService
{
	private static Settings? _settings;

	/// <summary>
	/// Aim: Get the current settings, loading from prefs if needed.
	/// </summary>
	/// <returns>The settings object.</returns>
	public static Settings Get()
	{
		if (_settings != null)
		{
			return _settings;
		}

		_settings = PrefsHelper.GetValue<Settings>(MobileConstants.PREFS_SETTINGS);
		if (_settings == null)
		{
			_settings = new Settings();
			Save();
		}

		return _settings;
	}

	/// <summary>
	/// Aim: Save the current settings to prefs.
	/// </summary>
	public static void Save()
	{
		if (_settings != null)
		{
			PrefsHelper.SetValue(MobileConstants.PREFS_SETTINGS, _settings);
		}
	}

	/// <summary>
	/// Aim: Update settings and save.
	/// </summary>
	/// <param name="settings">The settings to save.</param>
	public static void Update(Settings settings)
	{
		_settings = settings;
		Save();
	}

	/// <summary>
	/// Aim: Check if settings have been initialized (saved to prefs by user).
	/// </summary>
	/// <returns>True if initialized, false otherwise.</returns>
	public static bool IsInitialized()
	{
		bool startAlwaysWithWelcome = MobileConstants.START_ALWAYS_WITH_WELCOME;
        if (startAlwaysWithWelcome)
			return false;

		bool settingsInitialized = PrefsHelper.GetValue<bool>(MobileConstants.PREFS_SETTINGS_INITIALIZED);
		return settingsInitialized;
	}

	/// <summary>
	/// Aim: Clear the cached settings (used when prefs are cleared).
	/// </summary>
	public static void ClearCache()
	{
		_settings = null;
	}
}

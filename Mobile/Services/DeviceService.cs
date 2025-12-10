#region Usings
using Microsoft.Maui.ApplicationModel;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Provides device-related functionality like theme detection and permission handling
/// </summary>
public static class DeviceService
{
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
		AppTheme theme = GetSystemTheme();
		bool isDark = theme == AppTheme.Dark;
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
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> CheckContactsReadPermissionAsync()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
	}

	/// <summary>
	/// Aim: Requests contacts read permission
	/// Return: True if granted, false otherwise
	/// </summary>
	public static async Task<bool> RequestContactsReadPermissionAsync()
	{
		PermissionStatus status = await Permissions.RequestAsync<Permissions.ContactsRead>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
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
		PermissionStatus status = await Permissions.RequestAsync<Permissions.ContactsWrite>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
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
		PermissionStatus status = await Permissions.RequestAsync<Permissions.CalendarRead>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
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
		PermissionStatus status = await Permissions.RequestAsync<Permissions.CalendarWrite>();
		bool isGranted = status == PermissionStatus.Granted;
		return isGranted;
	}

	/// <summary>
	/// Aim: Shows a permission denied alert and optionally opens settings
	/// Params: title - Alert title, message - Alert message
	/// </summary>
	public static async Task ShowPermissionDeniedAlertAsync(string title, string message)
	{
		bool openSettings = await Application.Current!.MainPage!.DisplayAlert(
			title,
			message,
			MobileLanguages.Resources.General_Button_OK,
			MobileLanguages.Resources.General_Button_Cancel);

		if (openSettings)
		{
			AppInfo.ShowSettingsUI();
		}
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

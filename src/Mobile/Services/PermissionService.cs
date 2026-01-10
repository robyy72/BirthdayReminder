#region Usings
#endregion

namespace Mobile;

/// <summary>
/// Aim: Centralized service for handling permission state changes.
/// </summary>
public static class PermissionService
{
	#region Private Fields
	private static AppPermissionStatus _lastContactsPermission;
	private static AppPermissionStatus _lastCalendarPermission;
	#endregion

	#region Events
	public static event Action<AppPermissionStatus>? ContactsPermissionChanged;
	public static event Action<AppPermissionStatus>? CalendarPermissionChanged;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Initialize permission tracking with current state from Account.
	/// </summary>
	public static void InitTracking()
	{
		_lastContactsPermission = App.Account.ContactsPermission;
		_lastCalendarPermission = App.Account.CalendarPermission;
	}

	/// <summary>
	/// Aim: Check if contacts permission changed and update Account if so.
	/// </summary>
	public static async Task CheckContactsPermissionAsync()
	{
		bool isGranted = await DeviceService.CheckContactsReadPermissionAsync();
		AppPermissionStatus currentStatus = isGranted ? AppPermissionStatus.Granted : AppPermissionStatus.Denied;

		if (currentStatus != _lastContactsPermission)
		{
			await UpdateContactsPermissionAsync(currentStatus);
			ContactsPermissionChanged?.Invoke(currentStatus);
		}
	}

	/// <summary>
	/// Aim: Update contacts permission status and related Account settings.
	/// Params: status - The new permission status.
	/// </summary>
	public static async Task UpdateContactsPermissionAsync(AppPermissionStatus status)
	{
		_lastContactsPermission = status;
		App.Account.ContactsPermission = status;

		if (status == AppPermissionStatus.Denied)
		{
			App.Account.ContactsReadMode = ContactsReadMode.None;
			App.Account.ContactsReadWriteMode = ContactsReadWriteMode.NotSet;
			App.Account.ContactsAccessChoice = ContactsAccessChoice.NotSet;
		}
		else
		{
			App.Account.ContactsAccessChoice = await DeviceService.GetContactsAccessChoiceAsync();
		}

		AccountService.Save();
	}

	/// <summary>
	/// Aim: Check if calendar permission changed and update Account if so.
	/// </summary>
	public static async Task CheckCalendarPermissionAsync()
	{
		bool isGranted = await DeviceService.CheckCalendarReadPermissionAsync();
		AppPermissionStatus currentStatus = isGranted ? AppPermissionStatus.Granted : AppPermissionStatus.Denied;

		if (currentStatus != _lastCalendarPermission)
		{
			UpdateCalendarPermission(currentStatus);
			CalendarPermissionChanged?.Invoke(currentStatus);
		}
	}

	/// <summary>
	/// Aim: Update calendar permission status in Account.
	/// Params: status - The new permission status.
	/// </summary>
	public static void UpdateCalendarPermission(AppPermissionStatus status)
	{
		_lastCalendarPermission = status;
		App.Account.CalendarPermission = status;
		AccountService.Save();
	}
	#endregion
}

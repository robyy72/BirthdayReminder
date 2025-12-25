namespace Mobile;

/// <summary>
/// Aim: Service for managing application account/settings.
/// </summary>
public static class AccountService
{
	/// <summary>
	/// Aim: Get the current account, loading from prefs if needed.
	/// </summary>
	/// <returns>The account object.</returns>
	public static void Load()
	{
		Account? account = PrefsHelper.GetValue<Account>(MobileConstants.PREFS_ACCOUNT);
		if (account != null)
		{
			App.Account = account;
        }
	}

	/// <summary>
	/// Aim: Save the current account to prefs.
	/// </summary>
	public static void Save()
	{
		if (App.Account != null)
		{
			PrefsHelper.SetValue(MobileConstants.PREFS_ACCOUNT, App.Account);
		}
	}

	/// <summary>
	/// Aim: Check if account has been initialized (saved to prefs by user).
	/// </summary>
	/// <returns>True if initialized, false otherwise.</returns>
	public static bool IsInitialized()
	{
		bool startAlwaysWithWelcome = MobileConstants.START_ALWAYS_WITH_WELCOME;
        if (startAlwaysWithWelcome)
			return false;

		bool accountInitialized = PrefsHelper.GetValue<bool>(MobileConstants.PREFS_ACCOUNT_INITIALIZED);
		return accountInitialized;
	}

	/// <summary>
	/// Aim: Check if contacts are being used.
	/// Return: True if ContactsReadMode is not None
	/// </summary>
	public static bool UseContacts()
	{
		bool result = App.Account.ContactsReadMode > ContactsReadMode.None;
		return result;
	}

	/// <summary>
	/// Aim: Checks if permissions (contacts, calendar) are still granted, updates Account settings if not.
	/// </summary>
	public static async void CheckRightsAndUpdateAccount()
	{
		bool needsSave = false;

		// Check Contacts permission
		if (App.Account.ContactsReadMode != ContactsReadMode.None && App.Account.ContactsReadMode != ContactsReadMode.NotSet)
		{
			bool hasContactsRead = await DeviceService.CheckContactsReadPermissionAsync();
			if (!hasContactsRead)
			{
				App.Account.ContactsReadMode = ContactsReadMode.None;
				needsSave = true;
			}
		}

		// Check Calendar permission
		if (App.Account.DeviceCalendar_Enabled)
		{
			bool hasCalendarRead = await DeviceService.CheckCalendarReadPermissionAsync();
			if (!hasCalendarRead)
			{
				App.Account.DeviceCalendar_Enabled = false;
				App.Account.DeviceCalendar_SelectedIds = [];
				needsSave = true;
			}
		}

		if (needsSave)
			Save();
	}
}

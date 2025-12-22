namespace Mobile;

/// <summary>
/// Aim: Service for managing application account/settings.
/// </summary>
public static class AccountService
{
	private static Account? _account;

	/// <summary>
	/// Aim: Get the current account, loading from prefs if needed.
	/// </summary>
	/// <returns>The account object.</returns>
	public static Account Get()
	{
		if (_account != null)
		{
			return _account;
		}

		_account = PrefsHelper.GetValue<Account>(MobileConstants.PREFS_ACCOUNT);
		if (_account == null)
		{
			_account = new Account();
			Save();
		}

		return _account;
	}

	/// <summary>
	/// Aim: Save the current account to prefs.
	/// </summary>
	public static void Save()
	{
		if (_account != null)
		{
			PrefsHelper.SetValue(MobileConstants.PREFS_ACCOUNT, _account);
		}
	}

	/// <summary>
	/// Aim: Update account and save.
	/// </summary>
	/// <param name="account">The account to save.</param>
	public static void Update(Account account)
	{
		_account = account;
		Save();
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
	/// Aim: Clear the cached account (used when prefs are cleared).
	/// </summary>
	public static void ClearCache()
	{
		_account = null;
	}
}

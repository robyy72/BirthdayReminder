namespace Mobile;

public partial class StartPage_3 : ContentPage
{
	private bool _isLoading = true;

	public StartPage_3()
	{
		InitializeComponent();
		LoadSettings();
		_isLoading = false;
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		if (settings.ContactsMode == ContactsMode.None)
			RadioContactsNone.IsChecked = true;
		else if (settings.ContactsMode == ContactsMode.ReadWrite)
			RadioContactsReadWrite.IsChecked = true;
		else if (settings.ContactsMode == ContactsMode.BirthdayCalendar)
			RadioContactsBirthdayCalendar.IsChecked = true;
		else
			RadioContactsRead.IsChecked = true;
	}

	private async void OnContactsRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (!e.Value || _isLoading)
			return;

		if (sender == RadioContactsRead || sender == RadioContactsReadWrite)
		{
			bool granted = await DeviceService.RequestContactsReadPermissionAsync();
			if (!granted)
			{
				RadioContactsNone.IsChecked = true;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
				return;
			}

			if (sender == RadioContactsReadWrite)
			{
				bool writeGranted = await DeviceService.RequestContactsWritePermissionAsync();
				if (!writeGranted)
				{
					RadioContactsRead.IsChecked = true;
					await DisplayAlert(
						MobileLanguages.Resources.Permission_Required,
						MobileLanguages.Resources.Permission_ContactsWrite_Denied,
						MobileLanguages.Resources.General_Button_OK);
				}
			}
		}
		else if (sender == RadioContactsBirthdayCalendar)
		{
			bool granted = await DeviceService.RequestCalendarReadPermissionAsync();
			if (!granted)
			{
				RadioContactsNone.IsChecked = true;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_2();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		if (RadioContactsNone.IsChecked)
			settings.ContactsMode = ContactsMode.None;
		else if (RadioContactsReadWrite.IsChecked)
			settings.ContactsMode = ContactsMode.ReadWrite;
		else if (RadioContactsBirthdayCalendar.IsChecked)
			settings.ContactsMode = ContactsMode.BirthdayCalendar;
		else
			settings.ContactsMode = ContactsMode.Read;

		SettingsService.Update(settings);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_4();
		}
	}
}

namespace Mobile;

public partial class SettingsPage : ContentPage
{
	private int _hours = 18;
	private int _minutes = 0;

	public SettingsPage()
	{
		InitializeComponent();
		LoadSettings();
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		_hours = settings.DefaultReminderTime / 100;
		_minutes = settings.DefaultReminderTime % 100;

		HoursEntry.Text = _hours.ToString("D2");
		MinutesEntry.Text = _minutes.ToString("D2");

		if (settings.Locale == "en")
		{
			RadioEn.IsChecked = true;
		}
		else
		{
			RadioDe.IsChecked = true;
		}

		ReadContactsBirthdaySwitch.IsToggled = settings.ReadFromContactsWithBirthday;
		ReadContactsAllSwitch.IsToggled = settings.ReadFromContactsAll;
		WriteContactsSwitch.IsToggled = settings.WriteToContacts;
		ReadCalendarsSwitch.IsToggled = settings.ReadFromCalenders;
		WriteCalendarsSwitch.IsToggled = settings.WriteToCalenders;
	}

	private void OnHoursUp(object? sender, EventArgs e)
	{
		_hours = (_hours + 1) % 24;
		HoursEntry.Text = _hours.ToString("D2");
	}

	private void OnHoursDown(object? sender, EventArgs e)
	{
		_hours = (_hours - 1 + 24) % 24;
		HoursEntry.Text = _hours.ToString("D2");
	}

	private void OnMinutesUp(object? sender, EventArgs e)
	{
		_minutes = (_minutes + 5) % 60;
		MinutesEntry.Text = _minutes.ToString("D2");
	}

	private void OnMinutesDown(object? sender, EventArgs e)
	{
		_minutes = (_minutes - 5 + 60) % 60;
		MinutesEntry.Text = _minutes.ToString("D2");
	}

	private async void OnContactsReadToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.ContactsRead>();
			if (status != PermissionStatus.Granted)
			{
				if (sender is Switch sw)
				{
					sw.IsToggled = false;
				}
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private async void OnContactsWriteToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		var status = await Permissions.CheckStatusAsync<Permissions.ContactsWrite>();
		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.ContactsWrite>();
			if (status != PermissionStatus.Granted)
			{
				WriteContactsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsWrite_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private async void OnCalendarReadToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		var status = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.CalendarRead>();
			if (status != PermissionStatus.Granted)
			{
				ReadCalendarsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private async void OnCalendarWriteToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		var status = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();
		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.CalendarWrite>();
			if (status != PermissionStatus.Granted)
			{
				WriteCalendarsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarWrite_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		if (int.TryParse(HoursEntry.Text, out int hours))
		{
			_hours = Math.Clamp(hours, 0, 23);
		}
		if (int.TryParse(MinutesEntry.Text, out int minutes))
		{
			_minutes = Math.Clamp(minutes, 0, 59);
		}

		settings.DefaultReminderTime = _hours * 100 + _minutes;

		if (RadioEn.IsChecked)
		{
			settings.Locale = "en";
		}
		else
		{
			settings.Locale = "de";
		}

		settings.ReadFromContactsWithBirthday = ReadContactsBirthdaySwitch.IsToggled;
		settings.ReadFromContactsAll = ReadContactsAllSwitch.IsToggled;
		settings.WriteToContacts = WriteContactsSwitch.IsToggled;
		settings.ReadFromCalenders = ReadCalendarsSwitch.IsToggled;
		settings.WriteToCalenders = WriteCalendarsSwitch.IsToggled;

		SettingsService.Update(settings);

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
	}
}

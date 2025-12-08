namespace Mobile;

public partial class Welcome_2Page : ContentPage
{
	private bool _contactsReadPermissionGranted = false;
	private bool _contactsWritePermissionGranted = false;
	private bool _calendarReadPermissionGranted = false;
	private bool _calendarWritePermissionGranted = false;

	public Welcome_2Page()
	{
		InitializeComponent();
		LoadSettings();
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		ReadContactsBirthdaySwitch.IsToggled = settings.ReadFromContactsWithBirthday;
		ReadContactsAllSwitch.IsToggled = settings.ReadFromContactsAll;
		WriteContactsSwitch.IsToggled = settings.WriteToContacts;
		ReadCalendarsSwitch.IsToggled = settings.ReadFromCalenders;
		WriteCalendarsSwitch.IsToggled = settings.WriteToCalenders;

		UpdateCalendarListVisibility();
	}

	private async void OnContactsReadToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		if (!_contactsReadPermissionGranted)
		{
			var status = await Permissions.RequestAsync<Permissions.ContactsRead>();
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
				return;
			}
			_contactsReadPermissionGranted = true;
		}
	}

	private async void OnContactsWriteToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		if (!_contactsWritePermissionGranted)
		{
			var status = await Permissions.RequestAsync<Permissions.ContactsWrite>();
			if (status != PermissionStatus.Granted)
			{
				WriteContactsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsWrite_Denied,
					MobileLanguages.Resources.General_Button_OK);
				return;
			}
			_contactsWritePermissionGranted = true;
		}
	}

	private async void OnCalendarReadToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			UpdateCalendarListVisibility();
			return;
		}

		if (!_calendarReadPermissionGranted)
		{
			var status = await Permissions.RequestAsync<Permissions.CalendarRead>();
			if (status != PermissionStatus.Granted)
			{
				ReadCalendarsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
				return;
			}
			_calendarReadPermissionGranted = true;
		}

		UpdateCalendarListVisibility();
		await LoadCalendarsAsync();
	}

	private async void OnCalendarWriteToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			UpdateCalendarListVisibility();
			return;
		}

		if (!_calendarWritePermissionGranted)
		{
			var status = await Permissions.RequestAsync<Permissions.CalendarWrite>();
			if (status != PermissionStatus.Granted)
			{
				WriteCalendarsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarWrite_Denied,
					MobileLanguages.Resources.General_Button_OK);
				return;
			}
			_calendarWritePermissionGranted = true;
		}

		UpdateCalendarListVisibility();
		await LoadCalendarsAsync();
	}

	private void UpdateCalendarListVisibility()
	{
		CalendarListContainer.IsVisible = ReadCalendarsSwitch.IsToggled || WriteCalendarsSwitch.IsToggled;
	}

	private async Task LoadCalendarsAsync()
	{
		try
		{
			var calendars = await GetDeviceCalendarsAsync();
			CalendarListView.ItemsSource = calendars;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error loading calendars: {ex.Message}");
		}
	}

	private Task<List<CalendarInfo>> GetDeviceCalendarsAsync()
	{
		var calendars = new List<CalendarInfo>
		{
			new CalendarInfo { Id = "default", Name = MobileLanguages.Resources.Calendar_Default, Color = Colors.Blue },
			new CalendarInfo { Id = "personal", Name = MobileLanguages.Resources.Calendar_Personal, Color = Colors.Green },
			new CalendarInfo { Id = "work", Name = MobileLanguages.Resources.Calendar_Work, Color = Colors.Orange },
			new CalendarInfo { Id = "family", Name = MobileLanguages.Resources.Calendar_Family, Color = Colors.Purple }
		};

		return Task.FromResult(calendars);
	}

	private void OnCalendarSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//Welcome_1Page");
	}

	private void OnFinishClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		settings.ReadFromContactsWithBirthday = ReadContactsBirthdaySwitch.IsToggled;
		settings.ReadFromContactsAll = ReadContactsAllSwitch.IsToggled;
		settings.WriteToContacts = WriteContactsSwitch.IsToggled;
		settings.ReadFromCalenders = ReadCalendarsSwitch.IsToggled;
		settings.WriteToCalenders = WriteCalendarsSwitch.IsToggled;

		SettingsService.Update(settings);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}
}

public class CalendarInfo
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public Color Color { get; set; } = Colors.Blue;
}

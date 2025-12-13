namespace Mobile;

public partial class SettingsPage : ContentPage
{
	private readonly List<string> _hoursList = [];
	private readonly List<string> _minutesList = [];
	private List<CalendarInfo> _calendars = [];
	private bool _isLoading = true;

	public SettingsPage()
	{
		InitializeComponent();
		InitializePickers();
		LoadSettings();
		_isLoading = false;
	}

	private void InitializePickers()
	{
		for (int i = 0; i < 24; i++)
			_hoursList.Add(i.ToString("D2"));

		for (int i = 0; i < 60; i++)
			_minutesList.Add(i.ToString("D2"));

		HoursPicker.ItemsSource = _hoursList;
		MinutesPicker.ItemsSource = _minutesList;
	}

	private void LoadSettings()
	{
		var settings = SettingsService.Get();

		UpcomingEntry.Text = settings.ShowUpcomingBirthdays.ToString();
		PastEntry.Text = settings.ShowPastBirthdays.ToString();

		int hours = settings.DefaultReminderTime / 100;
		int minutes = settings.DefaultReminderTime % 100;

		HoursPicker.SelectedIndex = hours;
		MinutesPicker.SelectedIndex = minutes;

		RemindUntilApprovedSwitch.IsToggled = settings.RemindUntilApproved;

		if (settings.Locale == "en")
			RadioEn.IsChecked = true;
		else
			RadioDe.IsChecked = true;

		switch (settings.ContactsMode)
		{
			case ContactsMode.None:
				RadioContactsNone.IsChecked = true;
				break;
			case ContactsMode.ReadWrite:
				RadioContactsReadWrite.IsChecked = true;
				break;
			case ContactsMode.BirthdayCalendar:
				RadioContactsBirthdayCalendar.IsChecked = true;
				break;
			default:
				RadioContactsRead.IsChecked = true;
				break;
		}

		WriteCalendarsSwitch.IsToggled = settings.WriteToCalendars;
		UpdateCalendarListVisibility();

		if (settings.WriteToCalendars)
		{
			_ = LoadCalendarsAsync(settings.SelectedCalendarIds);
		}
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

	private async void OnWriteCalendarsToggled(object? sender, ToggledEventArgs e)
	{
		if (_isLoading)
			return;

		if (e.Value)
		{
			bool granted = await DeviceService.RequestCalendarWritePermissionAsync();
			if (!granted)
			{
				WriteCalendarsSwitch.IsToggled = false;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarWrite_Denied,
					MobileLanguages.Resources.General_Button_OK);
				return;
			}

			await LoadCalendarsAsync([]);
		}

		UpdateCalendarListVisibility();
	}

	private void UpdateCalendarListVisibility()
	{
		CalendarListContainer.IsVisible = WriteCalendarsSwitch.IsToggled;
	}

	private async Task LoadCalendarsAsync(List<string> selectedIds)
	{
		try
		{
			_calendars = await GetDeviceCalendarsAsync();
			foreach (var cal in _calendars)
			{
				cal.IsSelected = selectedIds.Contains(cal.Id);
			}
			BuildCalendarToggles();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error loading calendars: {ex.Message}");
		}
	}

	private Task<List<CalendarInfo>> GetDeviceCalendarsAsync()
	{
		// TODO: Replace with actual calendar API
		var calendars = new List<CalendarInfo>
		{
			new() { Id = "default", Name = MobileLanguages.Resources.Calendar_Default, Color = Colors.Blue },
			new() { Id = "personal", Name = MobileLanguages.Resources.Calendar_Personal, Color = Colors.Green },
			new() { Id = "work", Name = MobileLanguages.Resources.Calendar_Work, Color = Colors.Orange },
			new() { Id = "family", Name = MobileLanguages.Resources.Calendar_Family, Color = Colors.Purple }
		};

		return Task.FromResult(calendars);
	}

	private void BuildCalendarToggles()
	{
		CalendarTogglesContainer.Children.Clear();

		foreach (var calendar in _calendars)
		{
			var grid = new Grid
			{
				ColumnDefinitions =
				[
					new ColumnDefinition(GridLength.Auto),
					new ColumnDefinition(GridLength.Star),
					new ColumnDefinition(GridLength.Auto)
				],
				Padding = new Thickness(5, 0)
			};

			var colorBox = new BoxView
			{
				WidthRequest = 12,
				HeightRequest = 12,
				CornerRadius = 6,
				Color = calendar.Color,
				VerticalOptions = LayoutOptions.Center
			};
			grid.SetColumn(colorBox, 0);

			var label = new Label
			{
				Text = calendar.Name,
				FontSize = 16,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(10, 0, 0, 0)
			};
			grid.SetColumn(label, 1);

			var toggle = new Switch
			{
				IsToggled = calendar.IsSelected,
				VerticalOptions = LayoutOptions.Center
			};
			toggle.Toggled += (s, e) => calendar.IsSelected = e.Value;
			grid.SetColumn(toggle, 2);

			grid.Children.Add(colorBox);
			grid.Children.Add(label);
			grid.Children.Add(toggle);

			CalendarTogglesContainer.Children.Add(grid);
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var settings = SettingsService.Get();

		if (int.TryParse(UpcomingEntry.Text, out int upcoming))
			settings.ShowUpcomingBirthdays = Math.Clamp(upcoming, 1, MobileConstants.SHOW_MAX_BIRTHDAYS);

		if (int.TryParse(PastEntry.Text, out int past))
			settings.ShowPastBirthdays = Math.Clamp(past, 1, MobileConstants.SHOW_MAX_BIRTHDAYS);

		int hours = HoursPicker.SelectedIndex;
		int minutes = MinutesPicker.SelectedIndex;
		settings.DefaultReminderTime = hours * 100 + minutes;

		settings.RemindUntilApproved = RemindUntilApprovedSwitch.IsToggled;

		if (RadioEn.IsChecked)
			settings.Locale = "en";
		else
			settings.Locale = "de";

		if (RadioContactsNone.IsChecked)
			settings.ContactsMode = ContactsMode.None;
		else if (RadioContactsReadWrite.IsChecked)
			settings.ContactsMode = ContactsMode.ReadWrite;
		else if (RadioContactsBirthdayCalendar.IsChecked)
			settings.ContactsMode = ContactsMode.BirthdayCalendar;
		else
			settings.ContactsMode = ContactsMode.Read;

		settings.WriteToCalendars = WriteCalendarsSwitch.IsToggled;
		settings.SelectedCalendarIds = _calendars
			.Where(c => c.IsSelected)
			.Select(c => c.Id)
			.ToList();

		SettingsService.Update(settings);

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
	}
}

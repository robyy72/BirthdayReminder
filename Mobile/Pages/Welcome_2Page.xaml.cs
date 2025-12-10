namespace Mobile;

public partial class Welcome_2Page : ContentPage
{
	private List<CalendarInfo> _calendars = [];
	private bool _isLoading = true;

	public Welcome_2Page()
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

		WriteCalendarsSwitch.IsToggled = settings.WriteToCalendars;
		UpdateCalendarListVisibility();
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

			await LoadCalendarsAsync();
		}

		UpdateCalendarListVisibility();
	}

	private void UpdateCalendarListVisibility()
	{
		CalendarListContainer.IsVisible = WriteCalendarsSwitch.IsToggled;
	}

	private async Task LoadCalendarsAsync()
	{
		try
		{
			_calendars = await GetDeviceCalendarsAsync();
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
		// Birthday calendar is excluded - it's a special one
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

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new Welcome_1Page();
		}
	}

	private void OnFinishClicked(object? sender, EventArgs e)
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

		settings.WriteToCalendars = WriteCalendarsSwitch.IsToggled;
		settings.SelectedCalendarIds = _calendars
			.Where(c => c.IsSelected)
			.Select(c => c.Id)
			.ToList();

		SettingsService.Update(settings);

		PrefsHelper.SetValue(MobileConstants.PREFS_SETTINGS_INITIALIZED, true);

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
	public bool IsSelected { get; set; } = false;
}

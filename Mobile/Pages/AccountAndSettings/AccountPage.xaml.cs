namespace Mobile;

public partial class AccountPage : ContentPage
{
	#region Fields
	private List<CalendarInfo> _calendars = [];
	private bool _isLoading = true;
	#endregion

	#region Constructor
	public AccountPage()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}
	#endregion

	#region Load
	private void LoadAccount()
	{
		var account = App.Account;

		UpcomingEntry.Text = account.ShowUpcomingBirthdays.ToString();
		PastEntry.Text = account.ShowPastBirthdays.ToString();

		if (account.Theme == "Dark")
			RadioDark.IsChecked = true;
		else
			RadioLight.IsChecked = true;

		if (account.Locale == "en")
			RadioEn.IsChecked = true;
		else
			RadioDe.IsChecked = true;

		if (account.ContactsReadMode == ContactsReadMode.None || account.ContactsReadMode == ContactsReadMode.NotSet)
			RadioContactsNone.IsChecked = true;
		else
			RadioContactsRead.IsChecked = true;

		WriteCalendarsSwitch.IsToggled = account.DeviceCalendar_Enabled;
		UpdateCalendarListVisibility();

		if (account.DeviceCalendar_Enabled)
		{
			_ = LoadCalendarsAsync(account.DeviceCalendar_SelectedIds);
		}
	}
	#endregion

	#region Contacts
	private async void OnContactsRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (!e.Value || _isLoading)
			return;

		if (sender == RadioContactsRead)
		{
			bool granted = await DeviceService.RequestContactsReadPermissionAsync();
			if (!granted)
			{
				RadioContactsNone.IsChecked = true;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}
	#endregion

	#region Calendars
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
	#endregion

	#region Save
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var account = App.Account;

		if (int.TryParse(UpcomingEntry.Text, out int upcoming))
			account.ShowUpcomingBirthdays = Math.Clamp(upcoming, 1, MobileConstants.SHOW_MAX_BIRTHDAYS);

		if (int.TryParse(PastEntry.Text, out int past))
			account.ShowPastBirthdays = Math.Clamp(past, 1, MobileConstants.SHOW_MAX_BIRTHDAYS);

		if (RadioDark.IsChecked)
			account.Theme = "Dark";
		else
			account.Theme = "Light";

		DeviceService.ApplyTheme(account.Theme);

		if (RadioEn.IsChecked)
			account.Locale = "en";
		else
			account.Locale = "de";

		if (RadioContactsNone.IsChecked)
			account.ContactsReadMode = ContactsReadMode.None;
		else if (RadioContactsRead.IsChecked)
		{
			// If enabling contacts, default to ReadNamesWithBirthday if not already set
			if (account.ContactsReadMode == ContactsReadMode.None || account.ContactsReadMode == ContactsReadMode.NotSet)
				account.ContactsReadMode = ContactsReadMode.ReadNamesWithBirthday;
		}

		account.DeviceCalendar_Enabled = WriteCalendarsSwitch.IsToggled;
		account.DeviceCalendar_SelectedIds = _calendars
			.Where(c => c.IsSelected)
			.Select(c => c.Id)
			.ToList();

		AccountService.Save();

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
	}
	#endregion

	#region Back
	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await NavigationService.GoBack();
	}
	#endregion
}

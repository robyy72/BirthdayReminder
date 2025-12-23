namespace Mobile;

public partial class StartPage_9 : ContentPage
{
	private List<CalendarInfo> _calendars = [];
	private bool _isLoading = true;
	private int _openAccordion = 0;

	public StartPage_9()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}

	private void LoadAccount()
	{
		var account = App.Account;

		// Device Calendar
		DeviceCalendarSwitch.IsToggled = account.DeviceCalendar_Enabled;
		UpdateCalendarListVisibility();

		// Outlook
		OutlookSingleEmailSwitch.IsToggled = account.OutlookCalendar_SingleEmail;
		OutlookGrantAccessSwitch.IsToggled = account.OutlookCalendar_GrantAccess;

		// Google
		GoogleSingleEmailSwitch.IsToggled = account.GoogleCalendar_SingleEmail;
		GoogleGrantAccessSwitch.IsToggled = account.GoogleCalendar_GrantAccess;
	}

	#region Accordion Logic
	private void OnAccordion1Tapped(object? sender, TappedEventArgs e)
	{
		ToggleAccordion(1);
	}

	private void OnAccordion2Tapped(object? sender, TappedEventArgs e)
	{
		ToggleAccordion(2);
	}

	private void OnAccordion3Tapped(object? sender, TappedEventArgs e)
	{
		ToggleAccordion(3);
	}

	private void ToggleAccordion(int accordionNumber)
	{
		if (_openAccordion == accordionNumber)
		{
			// Close current accordion
			SetAccordionState(accordionNumber, false);
			_openAccordion = 0;
		}
		else
		{
			// Close previously open accordion
			if (_openAccordion > 0)
				SetAccordionState(_openAccordion, false);

			// Open new accordion
			SetAccordionState(accordionNumber, true);
			_openAccordion = accordionNumber;
		}
	}

	private void SetAccordionState(int accordionNumber, bool isOpen)
	{
		var content = accordionNumber switch
		{
			1 => Accordion1Content,
			2 => Accordion2Content,
			3 => Accordion3Content,
			_ => null
		};

		var arrow = accordionNumber switch
		{
			1 => Accordion1Arrow,
			2 => Accordion2Arrow,
			3 => Accordion3Arrow,
			_ => null
		};

		if (content != null)
			content.IsVisible = isOpen;

		if (arrow != null)
			arrow.Text = isOpen ? "▲" : "▼";
	}
	#endregion

	#region Device Calendar
	private async void OnDeviceCalendarToggled(object? sender, ToggledEventArgs e)
	{
		if (_isLoading)
			return;

		if (e.Value)
		{
			bool granted = await DeviceService.RequestCalendarWritePermissionAsync();
			if (!granted)
			{
				DeviceCalendarSwitch.IsToggled = false;
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
		CalendarListContainer.IsVisible = DeviceCalendarSwitch.IsToggled;
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

	#region Navigation
	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Page page = App.Account.ReminderCount switch
			{
				ReminderCount.ThreeReminders => new StartPage_8(),
				ReminderCount.TwoReminders => new StartPage_7(),
				ReminderCount.OneReminder => new StartPage_6(),
				_ => new StartPage_5()
			};
			Application.Current.Windows[0].Page = page;
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var account = App.Account;

		// Device Calendar
		account.DeviceCalendar_Enabled = DeviceCalendarSwitch.IsToggled;
		account.DeviceCalendar_SelectedIds = _calendars
			.Where(c => c.IsSelected)
			.Select(c => c.Id)
			.ToList();

		// Outlook
		account.OutlookCalendar_SingleEmail = OutlookSingleEmailSwitch.IsToggled;
		account.OutlookCalendar_GrantAccess = OutlookGrantAccessSwitch.IsToggled;

		// Google
		account.GoogleCalendar_SingleEmail = GoogleSingleEmailSwitch.IsToggled;
		account.GoogleCalendar_GrantAccess = GoogleGrantAccessSwitch.IsToggled;

		AccountService.Save();
		PrefsHelper.SetValue(MobileConstants.PREFS_ACCOUNT_INITIALIZED, true);

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new AppShell();
		}
	}
	#endregion
}

public class CalendarInfo
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public Color Color { get; set; } = Colors.Blue;
	public bool IsSelected { get; set; } = false;
}

namespace Mobile;

public partial class StartPage_8 : ContentPage
{
	private List<CalendarInfo> _calendars = [];
	private bool _isLoading = true;

	public StartPage_8()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}

	private void LoadAccount()
	{
		var account = App.Account;
		WriteCalendarsSwitch.IsToggled = account.WriteToCalendars;
		UpdateCalendarListVisibility();
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
			Page page = App.Account.ReminderCount switch
			{
				ReminderCount.ThreeReminders => new StartPage_7(),
				ReminderCount.TwoReminders => new StartPage_6(),
				ReminderCount.OneReminder => new StartPage_5(),
				_ => new StartPage_4()
			};
			Application.Current.Windows[0].Page = page;
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var account = App.Account;

		account.WriteToCalendars = WriteCalendarsSwitch.IsToggled;
		account.SelectedCalendarIds = _calendars
			.Where(c => c.IsSelected)
			.Select(c => c.Id)
			.ToList();

		AccountService.Save();
		PrefsHelper.SetValue(MobileConstants.PREFS_ACCOUNT_INITIALIZED, true);

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

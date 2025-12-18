#region Usings
#endregion

namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class CreateEditBirthdayPage : ContentPage
{
	private int? _personId;
	private Person? _existingPerson;

	private int _selectedDay = 1;
	private int _selectedMonth = 1;
	private int? _selectedYear = null;

	public string PersonId
	{
		set
		{
			if (int.TryParse(value, out var id))
			{
				_personId = id;
			}
		}
	}

	public CreateEditBirthdayPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (_personId.HasValue)
		{
			LoadPerson(_personId.Value);
		}
		else
		{
			SetDefaultValues();
		}

		UpdateDateLabel();
	}

	private void SetDefaultValues()
	{
		_selectedDay = DateTime.Now.Day;
		_selectedMonth = DateTime.Now.Month;
		_selectedYear = null;

		var settings = SettingsService.Get();
		ReminderEmailSwitch.IsToggled = settings.ReminderEmailEnabled;
		ReminderSmsSwitch.IsToggled = settings.ReminderSmsEnabled;
		ReminderLockScreenSwitch.IsToggled = settings.ReminderLockScreenEnabled;
		ReminderWhatsAppSwitch.IsToggled = settings.ReminderWhatsAppEnabled;
		RemindUntilApprovedSwitch.IsToggled = settings.RemindUntilApproved;
	}

	private void LoadPerson(int id)
	{
		_existingPerson = BirthdayService.GetPerson(id);
		if (_existingPerson != null)
		{
			FirstNameEntry.Text = _existingPerson.FirstName;
			LastNameEntry.Text = _existingPerson.LastName;

			if (_existingPerson.Birthday != null)
			{
				_selectedDay = _existingPerson.Birthday.Day;
				_selectedMonth = _existingPerson.Birthday.Month;
				_selectedYear = _existingPerson.Birthday.Year == 0 ? null : _existingPerson.Birthday.Year;
			}

			ReminderEmailSwitch.IsToggled = _existingPerson.ReminderEmailEnabled;
			ReminderSmsSwitch.IsToggled = _existingPerson.ReminderSmsEnabled;
			ReminderLockScreenSwitch.IsToggled = _existingPerson.ReminderLockScreenEnabled;
			ReminderWhatsAppSwitch.IsToggled = _existingPerson.ReminderWhatsAppEnabled;
			RemindUntilApprovedSwitch.IsToggled = _existingPerson.RemindUntilApproved;

			DeleteButton.IsVisible = true;
		}
	}

	private void UpdateDateLabel()
	{
		string monthName = GetMonthShortName(_selectedMonth);
		string yearText = _selectedYear.HasValue ? _selectedYear.Value.ToString() : "----";
		DateLabel.Text = $"{_selectedDay}. {monthName} {yearText}";
	}

	private static string GetMonthShortName(int month)
	{
		return month switch
		{
			1 => MobileLanguages.Resources.Month_Short_Jan,
			2 => MobileLanguages.Resources.Month_Short_Feb,
			3 => MobileLanguages.Resources.Month_Short_Mar,
			4 => MobileLanguages.Resources.Month_Short_Apr,
			5 => MobileLanguages.Resources.Month_Short_May,
			6 => MobileLanguages.Resources.Month_Short_Jun,
			7 => MobileLanguages.Resources.Month_Short_Jul,
			8 => MobileLanguages.Resources.Month_Short_Aug,
			9 => MobileLanguages.Resources.Month_Short_Sep,
			10 => MobileLanguages.Resources.Month_Short_Oct,
			11 => MobileLanguages.Resources.Month_Short_Nov,
			12 => MobileLanguages.Resources.Month_Short_Dec,
			_ => "---"
		};
	}

	private async void OnDateTapped(object? sender, EventArgs e)
	{
		var currentYear = DateTime.Now.Year;
		var initialDate = new DateTime(
			_selectedYear ?? currentYear,
			_selectedMonth,
			Math.Min(_selectedDay, DateTime.DaysInMonth(_selectedYear ?? currentYear, _selectedMonth))
		);

		string? action = await DisplayActionSheet(
			MobileLanguages.Resources.General_Label_Birthday,
			MobileLanguages.Resources.General_Button_Cancel,
			null,
			MobileLanguages.Resources.Birthday_WithYear,
			MobileLanguages.Resources.Birthday_WithoutYear
		);

		if (action == MobileLanguages.Resources.Birthday_WithYear)
		{
			await ShowDatePickerWithYear(initialDate);
		}
		else if (action == MobileLanguages.Resources.Birthday_WithoutYear)
		{
			await ShowDatePickerWithoutYear();
		}
	}

	private async Task ShowDatePickerWithYear(DateTime initialDate)
	{
		var result = await DisplayPromptAsync(
			MobileLanguages.Resources.General_Label_Birthday,
			MobileLanguages.Resources.Birthday_EnterDate,
			MobileLanguages.Resources.General_Button_OK,
			MobileLanguages.Resources.General_Button_Cancel,
			initialValue: initialDate.ToString("dd.MM.yyyy"),
			keyboard: Keyboard.Default
		);

		if (!string.IsNullOrEmpty(result) && DateTime.TryParse(result, out var parsedDate))
		{
			_selectedDay = parsedDate.Day;
			_selectedMonth = parsedDate.Month;
			_selectedYear = parsedDate.Year;
			UpdateDateLabel();
		}
	}

	private async Task ShowDatePickerWithoutYear()
	{
		var days = new List<string>();
		for (int i = 1; i <= 31; i++)
			days.Add(i.ToString());

		var months = new List<string>
		{
			MobileLanguages.Resources.Month_Short_Jan,
			MobileLanguages.Resources.Month_Short_Feb,
			MobileLanguages.Resources.Month_Short_Mar,
			MobileLanguages.Resources.Month_Short_Apr,
			MobileLanguages.Resources.Month_Short_May,
			MobileLanguages.Resources.Month_Short_Jun,
			MobileLanguages.Resources.Month_Short_Jul,
			MobileLanguages.Resources.Month_Short_Aug,
			MobileLanguages.Resources.Month_Short_Sep,
			MobileLanguages.Resources.Month_Short_Oct,
			MobileLanguages.Resources.Month_Short_Nov,
			MobileLanguages.Resources.Month_Short_Dec
		};

		string? dayResult = await DisplayActionSheet(
			MobileLanguages.Resources.General_Label_Day,
			MobileLanguages.Resources.General_Button_Cancel,
			null,
			days.ToArray()
		);

		if (string.IsNullOrEmpty(dayResult) || dayResult == MobileLanguages.Resources.General_Button_Cancel)
			return;

		string? monthResult = await DisplayActionSheet(
			MobileLanguages.Resources.General_Label_Month,
			MobileLanguages.Resources.General_Button_Cancel,
			null,
			months.ToArray()
		);

		if (string.IsNullOrEmpty(monthResult) || monthResult == MobileLanguages.Resources.General_Button_Cancel)
			return;

		_selectedDay = int.Parse(dayResult);
		_selectedMonth = months.IndexOf(monthResult) + 1;
		_selectedYear = null;
		UpdateDateLabel();
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
		var lastName = LastNameEntry.Text?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			return;

		var birthday = new Birthday
		{
			Day = _selectedDay,
			Month = _selectedMonth,
			Year = _selectedYear ?? 0
		};

		if (_existingPerson != null)
		{
			_existingPerson.FirstName = firstName;
			_existingPerson.LastName = lastName;
			_existingPerson.Birthday = birthday;
			_existingPerson.ReminderEmailEnabled = ReminderEmailSwitch.IsToggled;
			_existingPerson.ReminderSmsEnabled = ReminderSmsSwitch.IsToggled;
			_existingPerson.ReminderLockScreenEnabled = ReminderLockScreenSwitch.IsToggled;
			_existingPerson.ReminderWhatsAppEnabled = ReminderWhatsAppSwitch.IsToggled;
			_existingPerson.RemindUntilApproved = RemindUntilApprovedSwitch.IsToggled;
			BirthdayService.Update(_existingPerson);
			NotificationService.ScheduleForPerson(_existingPerson);
		}
		else
		{
			var person = new Person
			{
				Id = GenerateId(),
				FirstName = firstName,
				LastName = lastName,
				Birthday = birthday,
				ReminderEmailEnabled = ReminderEmailSwitch.IsToggled,
				ReminderSmsEnabled = ReminderSmsSwitch.IsToggled,
				ReminderLockScreenEnabled = ReminderLockScreenSwitch.IsToggled,
				ReminderWhatsAppEnabled = ReminderWhatsAppSwitch.IsToggled,
				RemindUntilApproved = RemindUntilApprovedSwitch.IsToggled
			};
			BirthdayService.Add(person);
			NotificationService.ScheduleForPerson(person);
		}

		App.NeedsReloadBirthdays = true;
		await Shell.Current.GoToAsync("..");
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_personId.HasValue)
		{
			NotificationService.CancelForPerson(_personId.Value);
			BirthdayService.Remove(_personId.Value);
			App.NeedsReloadBirthdays = true;
			await Shell.Current.GoToAsync("..");
		}
	}

	private static int GenerateId()
	{
		int id = (int)(DateTime.Now.Ticks % int.MaxValue);
		return id;
	}

	private void OnEntryFocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry)
		{
			Border? border = entry.Parent as Border;
			if (border != null)
			{
				border.Stroke = Color.FromArgb("#0066CC");
				border.StrokeThickness = 2;
			}
		}
	}

	private void OnEntryUnfocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry)
		{
			Border? border = entry.Parent as Border;
			if (border != null)
			{
				border.Stroke = Color.FromArgb("#ACACAC");
				border.StrokeThickness = 1;
			}
		}
	}
}

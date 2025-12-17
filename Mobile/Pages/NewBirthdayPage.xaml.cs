#region Usings
#endregion

namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class NewBirthdayPage : ContentPage
{
	private int? _personId;
	private Person? _existingPerson;

	private readonly List<string> _daysList = [];
	private readonly List<string> _monthsList = [];
	private readonly List<string> _yearsList = [];
	private bool _pickersInitialized = false;

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

	public NewBirthdayPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (!_pickersInitialized)
		{
			InitializePickers();
			_pickersInitialized = true;
		}
		if (_personId.HasValue)
		{
			LoadPerson(_personId.Value);
		}
		else
		{
			SetDefaultDate();
		}
	}

	private void InitializePickers()
	{
		for (int i = 1; i <= 31; i++)
			_daysList.Add(i.ToString());

		_monthsList.AddRange([
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
		]);

		_yearsList.Add("----");
		int currentYear = DateTime.Now.Year;
		for (int i = currentYear; i >= currentYear - 120; i--)
			_yearsList.Add(i.ToString());

		DayPicker.ItemsSource = _daysList;
		MonthPicker.ItemsSource = _monthsList;
		YearPicker.ItemsSource = _yearsList;
	}

	private void SetDefaultDate()
	{
		DayPicker.SelectedIndex = DateTime.Now.Day - 1;
		MonthPicker.SelectedIndex = DateTime.Now.Month - 1;
		YearPicker.SelectedIndex = 0;
		ReminderEmailSwitch.IsToggled = false;
		ReminderSmsSwitch.IsToggled = false;
		ReminderLockScreenSwitch.IsToggled = false;
		ReminderWhatsAppSwitch.IsToggled = false;
		RemindUntilApprovedSwitch.IsToggled = false;
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
				DayPicker.SelectedIndex = _existingPerson.Birthday.Day - 1;
				MonthPicker.SelectedIndex = _existingPerson.Birthday.Month - 1;

				if (_existingPerson.Birthday.Year == 0)
				{
					YearPicker.SelectedIndex = 0;
				}
				else
				{
					int currentYear = DateTime.Now.Year;
					int yearIndex = currentYear - _existingPerson.Birthday.Year + 1;
					YearPicker.SelectedIndex = Math.Clamp(yearIndex, 0, _yearsList.Count - 1);
				}
			}

			ReminderEmailSwitch.IsToggled = _existingPerson.ReminderEmailEnabled;
			ReminderSmsSwitch.IsToggled = _existingPerson.ReminderSmsEnabled;
			ReminderLockScreenSwitch.IsToggled = _existingPerson.ReminderLockScreenEnabled;
			ReminderWhatsAppSwitch.IsToggled = _existingPerson.ReminderWhatsAppEnabled;
			RemindUntilApprovedSwitch.IsToggled = _existingPerson.RemindUntilApproved;

			DeleteButton.IsVisible = true;
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
		var lastName = LastNameEntry.Text?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
			return;

		int day = DayPicker.SelectedIndex + 1;
		int month = MonthPicker.SelectedIndex + 1;
		int year = YearPicker.SelectedIndex == 0 ? 0 : int.Parse(_yearsList[YearPicker.SelectedIndex]);

		var birthday = new Birthday
		{
			Day = day,
			Month = month,
			Year = year
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

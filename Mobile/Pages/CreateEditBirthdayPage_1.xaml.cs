#region Usings
using Common;
#endregion

namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class CreateEditBirthdayPage_1 : ContentPage
{
	private int? _personId;
	private Person? _existingPerson;

	private List<string> _days = new();
	private List<string> _months = new();
	private List<string> _years = new();

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

	public CreateEditBirthdayPage_1()
	{
		InitializeComponent();
		InitializePickers();
	}

	private void InitializePickers()
	{
		for (int i = 1; i <= 31; i++)
		{
			_days.Add(i.ToString());
		}
		DayPicker.ItemsSource = _days;

		_months = new List<string>
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
		MonthPicker.ItemsSource = _months;

		int currentYear = DateTime.Now.Year;
		int minYear = currentYear - MobileConstants.MAX_AGE;
		_years.Add("0000");
		for (int year = currentYear; year >= minYear; year--)
		{
			_years.Add(year.ToString());
		}
		YearPicker.ItemsSource = _years;
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
	}

	private void SetDefaultValues()
	{
		DayPicker.SelectedIndex = DateTime.Now.Day - 1;
		MonthPicker.SelectedIndex = DateTime.Now.Month - 1;
		YearPicker.SelectedIndex = 0;

		NameDisplayPanel.IsVisible = false;
		NameEditPanel.IsVisible = true;

		WriteToContactRow.IsVisible = true;
		WriteToContactLabel.Text = MobileLanguages.Resources.Settings_CreateContact;
	}

	private void LoadPerson(int id)
	{
		_existingPerson = BirthdayService.GetPerson(id);
		if (_existingPerson != null)
		{
			FirstNameEntry.Text = _existingPerson.FirstName;
			LastNameEntry.Text = _existingPerson.LastName;

			string displayName = PersonHelper.GetDisplayName(
				_existingPerson.FirstName,
				_existingPerson.LastName,
				PersonNameDirection.FirstFirstName);
			NameDisplayLabel.Text = displayName;
			NameDisplayPanel.IsVisible = true;
			NameEditPanel.IsVisible = false;

			if (_existingPerson.Birthday != null)
			{
				DayPicker.SelectedIndex = _existingPerson.Birthday.Day - 1;
				MonthPicker.SelectedIndex = _existingPerson.Birthday.Month - 1;

				int year = _existingPerson.Birthday.Year;
				if (BirthdayHelper.ShouldDisplayYear(year))
				{
					int yearIndex = _years.IndexOf(year.ToString());
					YearPicker.SelectedIndex = yearIndex >= 0 ? yearIndex : 0;
				}
				else
				{
					YearPicker.SelectedIndex = 0;
				}
			}

			WriteToContactRow.IsVisible = true;
			if (!string.IsNullOrEmpty(_existingPerson.ContactId))
			{
				WriteToContactLabel.Text = MobileLanguages.Resources.Settings_UpdateContact;
			}
			else
			{
				WriteToContactLabel.Text = MobileLanguages.Resources.Settings_CreateContact;
			}
		}
	}

	private async void OnNameDisplayTapped(object? sender, EventArgs e)
	{
		await NameCard.RotateYTo(90, 150);
		NameDisplayPanel.IsVisible = false;
		NameEditPanel.IsVisible = true;
		await NameCard.RotateYTo(0, 150);
		FirstNameEntry.Focus();
	}

	private async void OnHelpBirthdayTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.BirthdayWithoutYear.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private async void OnNextClicked(object? sender, EventArgs e)
	{
		var firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
		var lastName = LastNameEntry.Text?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
		{
			return;
		}

		int selectedDay = DayPicker.SelectedIndex + 1;
		int selectedMonth = MonthPicker.SelectedIndex + 1;
		string selectedYearStr = _years[YearPicker.SelectedIndex];
		int selectedYear = selectedYearStr == "0000" ? 0 : int.Parse(selectedYearStr);

		var birthday = new Birthday
		{
			Day = selectedDay,
			Month = selectedMonth,
			Year = selectedYear
		};

		int personId;

		if (_existingPerson != null)
		{
			_existingPerson.FirstName = firstName;
			_existingPerson.LastName = lastName;
			_existingPerson.Birthday = birthday;
			BirthdayService.Update(_existingPerson);
			personId = _existingPerson.Id;
		}
		else
		{
			var settings = SettingsService.Get();
			var person = new Person
			{
				Id = GenerateId(),
				FirstName = firstName,
				LastName = lastName,
				Birthday = birthday,
				ReminderEmailEnabled = settings.ReminderEmailEnabled,
				ReminderSmsEnabled = settings.ReminderSmsEnabled,
				ReminderLockScreenEnabled = settings.ReminderLockScreenEnabled,
				ReminderWhatsAppEnabled = settings.ReminderWhatsAppEnabled,
				RemindUntilApproved = settings.RemindUntilApproved
			};
			BirthdayService.Add(person);
			personId = person.Id;
		}

		await Shell.Current.GoToAsync($"{nameof(CreateEditBirthdayPage_2)}?Id={personId}");
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

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}

	private async void OnDeleteMenuClicked(object? sender, EventArgs e)
	{
		if (_personId.HasValue)
		{
			await Shell.Current.GoToAsync($"{nameof(DeleteBirthdayPage)}?Id={_personId.Value}");
		}
	}

	private async void OnWriteToContactToggled(object? sender, ToggledEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		bool granted = await DeviceService.RequestContactsWritePermissionAsync();
		if (!granted)
		{
			WriteToContactSwitch.IsToggled = false;
			await DisplayAlert(
				MobileLanguages.Resources.Permission_Required,
				MobileLanguages.Resources.Permission_ContactsWrite_Denied,
				MobileLanguages.Resources.General_Button_OK);
		}
	}
}

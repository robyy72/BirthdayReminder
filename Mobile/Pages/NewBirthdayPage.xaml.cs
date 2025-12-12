#region Usings
using Common;
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
	private readonly List<string> _reminderTypeList = [];
	private readonly List<string> _reminderMethodList = [];
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
			MobileLanguages.Resources.Month_January,
			MobileLanguages.Resources.Month_February,
			MobileLanguages.Resources.Month_March,
			MobileLanguages.Resources.Month_April,
			MobileLanguages.Resources.Month_May,
			MobileLanguages.Resources.Month_June,
			MobileLanguages.Resources.Month_July,
			MobileLanguages.Resources.Month_August,
			MobileLanguages.Resources.Month_September,
			MobileLanguages.Resources.Month_October,
			MobileLanguages.Resources.Month_November,
			MobileLanguages.Resources.Month_December
		]);

		int currentYear = DateTime.Now.Year;
		for (int i = currentYear; i >= currentYear - 120; i--)
			_yearsList.Add(i.ToString());

		_reminderTypeList.AddRange([
			MobileLanguages.Resources.ReminderType_NotSet,
			MobileLanguages.Resources.ReminderType_DoNotRemind,
			MobileLanguages.Resources.ReminderType_RemindByMessage,
			MobileLanguages.Resources.ReminderType_RemindUntilApproved
		]);

		_reminderMethodList.AddRange([
			MobileLanguages.Resources.ReminderMethod_NotSet,
			MobileLanguages.Resources.ReminderMethod_Email,
			MobileLanguages.Resources.ReminderMethod_Sms,
			MobileLanguages.Resources.ReminderMethod_LockScreen,
			MobileLanguages.Resources.ReminderMethod_WhatsApp
		]);

		DayPicker.ItemsSource = _daysList;
		MonthPicker.ItemsSource = _monthsList;
		YearPicker.ItemsSource = _yearsList;
		ReminderTypePicker.ItemsSource = _reminderTypeList;
		ReminderMethodPicker.ItemsSource = _reminderMethodList;
	}

	private void SetDefaultDate()
	{
		DayPicker.SelectedIndex = DateTime.Now.Day - 1;
		MonthPicker.SelectedIndex = DateTime.Now.Month - 1;
		YearPicker.SelectedIndex = 0;
		ReminderTypePicker.SelectedIndex = 0;
		ReminderMethodPicker.SelectedIndex = 0;
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

				int currentYear = DateTime.Now.Year;
				int yearIndex = currentYear - _existingPerson.Birthday.Year;
				YearPicker.SelectedIndex = Math.Clamp(yearIndex, 0, _yearsList.Count - 1);
			}

			ReminderTypePicker.SelectedIndex = (int)_existingPerson.ReminderType;
			ReminderMethodPicker.SelectedIndex = (int)_existingPerson.ReminderMethod;
			UpdateReminderMethodVisibility();

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
		int year = int.Parse(_yearsList[YearPicker.SelectedIndex]);

		var birthday = new Birthday
		{
			Day = day,
			Month = month,
			Year = year
		};

		var reminderType = (ReminderType)ReminderTypePicker.SelectedIndex;
		var reminderMethod = (ReminderMethod)ReminderMethodPicker.SelectedIndex;

		if (_existingPerson != null)
		{
			_existingPerson.FirstName = firstName;
			_existingPerson.LastName = lastName;
			_existingPerson.Birthday = birthday;
			_existingPerson.ReminderType = reminderType;
			_existingPerson.ReminderMethod = reminderMethod;
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
				ReminderType = reminderType,
				ReminderMethod = reminderMethod
			};
			BirthdayService.Add(person);
			NotificationService.ScheduleForPerson(person);
		}

		App.NeedsReloadBirthdays = true;
		await Shell.Current.GoToAsync("..");
	}

	private void OnReminderTypeChanged(object? sender, EventArgs e)
	{
		UpdateReminderMethodVisibility();
	}

	private void UpdateReminderMethodVisibility()
	{
		var selectedType = (ReminderType)ReminderTypePicker.SelectedIndex;
		ReminderMethodContainer.IsVisible = selectedType == ReminderType.RemindByMessage;
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
}

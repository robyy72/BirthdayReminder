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

		DayPicker.ItemsSource = _daysList;
		MonthPicker.ItemsSource = _monthsList;
		YearPicker.ItemsSource = _yearsList;
	}

	private void SetDefaultDate()
	{
		DayPicker.SelectedItemsIndex = [DateTime.Now.Day - 1];
		MonthPicker.SelectedItemsIndex = [DateTime.Now.Month - 1];
		YearPicker.SelectedItemsIndex = [0];
	}

	private void LoadPerson(int id)
	{
		_existingPerson = BirthdayService.GetPerson(id);
		if (_existingPerson != null)
		{
			NameEntry.Text = _existingPerson.Name;
			if (_existingPerson.Birthday != null)
			{
				DayPicker.SelectedItemsIndex = [_existingPerson.Birthday.Day - 1];
				MonthPicker.SelectedItemsIndex = [_existingPerson.Birthday.Month - 1];

				int currentYear = DateTime.Now.Year;
				int yearIndex = currentYear - _existingPerson.Birthday.Year;
				YearPicker.SelectedItemsIndex = [Math.Clamp(yearIndex, 0, _yearsList.Count - 1)];
			}
			DeleteButton.IsVisible = true;
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var name = NameEntry.Text?.Trim();
		if (string.IsNullOrEmpty(name))
			return;

		int day = DayPicker.SelectedItemsIndex[0] + 1;
		int month = MonthPicker.SelectedItemsIndex[0] + 1;
		int year = int.Parse(_yearsList[YearPicker.SelectedItemsIndex[0]]);

		var birthday = new Birthday
		{
			Day = day,
			Month = month,
			Year = year
		};

		if (_existingPerson != null)
		{
			_existingPerson.Name = name;
			_existingPerson.Birthday = birthday;
			BirthdayService.Update(_existingPerson);
		}
		else
		{
			var person = new Person
			{
				Id = GenerateId(),
				Name = name,
				Birthday = birthday
			};
			BirthdayService.Add(person);
		}

		await Shell.Current.GoToAsync("..");
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_personId.HasValue)
		{
			BirthdayService.Remove(_personId.Value);
			await Shell.Current.GoToAsync("..");
		}
	}

	private static int GenerateId()
	{
		int id = (int)(DateTime.Now.Ticks % int.MaxValue);
		return id;
	}
}

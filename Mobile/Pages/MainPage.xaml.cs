namespace Mobile;

public partial class MainPage : ContentPage
{
    #region Properties
    public List<PersonViewModel> UpcomingBirthdays { get; set; } = [];
	public List<PersonViewModel> MissedBirthdays { get; set; } = [];
    #endregion

    #region Private Class Variables
    private List<Person> _persons = [];
	private Settings _settings = SettingsService.Get();
	private bool _foundContactsWithSameBirthday = false;
    #endregion


    #region Constructor and Start Methods
    public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
		Init();	
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		_settings = SettingsService.Get();
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, _settings.ShowUpcomingBirthdays);
		MissedBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, _settings.ShowPastBirthdays);
		if (App.NeedsReloadBirthdays)
		{
			LoadBirthdaysFromPrefs();
			App.NeedsReloadBirthdays = false;
		}
		UpdateBirthdayListsOnTheForm();
	}

	private void Init()
	{
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, _settings.ShowUpcomingBirthdays);
		MissedBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, _settings.ShowPastBirthdays);
		CheckRightsAndUpdateSettings();
		LoadBirthdaysFromPrefs();
		ReadContactsIfAllowed();
		ReadBirthdaysFromBirthdayCalenderIfAllowed();
	}
    #endregion

    #region Init Methods
    private async void CheckRightsAndUpdateSettings()
    {
        if (_settings.ContactsMode == ContactsMode.None)
            return;

        bool hasContactsRead = await DeviceService.CheckContactsReadPermissionAsync();
        if (!hasContactsRead && _settings.ContactsMode == ContactsMode.ReadFromContacts)
        {
            _settings.ContactsMode = ContactsMode.None;
            SettingsService.Update(_settings);
            return;
        }

        bool hasCalendarRead = await DeviceService.CheckCalendarReadPermissionAsync();
        if (!hasCalendarRead && _settings.ContactsMode == ContactsMode.BirthdayCalendar)
        {
            _settings.ContactsMode = ContactsMode.None;
            SettingsService.Update(_settings);
        }
    }

    private void LoadBirthdaysFromPrefs()
	{
		_persons.Clear();

		for (int month = 1; month <= 12; month++)		
			_persons.AddRange(BirthdayService.GetByMonth(month));		
	}
    
	private async void ReadContactsIfAllowed()
	{
		if (_settings.ContactsMode != ContactsMode.ReadFromContacts)
			return;

		try
		{
			ContactBirthdayService service = new();
			List<Person> persons = await service.GetContactsAsync();

			foreach (Person person in persons)
			{
				if (person.Birthday == null)
					continue;

				DateTime birthdayDate = BirthdayHelper.ConvertFromBirthdayToDateTime(person.Birthday);
				ImportContactBirthday(person.FirstName, person.LastName, birthdayDate, person.ContactId);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading contacts: {ex.Message}");
		}
	}

	/// <summary>
	/// Aim: Imports a contact with birthday into the persons list and saves to prefs
	/// Params: firstName - Contact first name, lastName - Contact last name, birthday - Contact birthday, contactId - Platform contact ID
	/// Return: True if added, false if duplicate
	/// </summary>
	public bool ImportContactBirthday(string firstName, string lastName, DateTime birthday, string? contactId)
	{
		if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
			return false;

		if (contactId != null)
		{
			var existingByContactId = _persons.FirstOrDefault(p => p.ContactId == contactId);
			if (existingByContactId != null)
				return false;
		}

		var sameBirthday = _persons.FirstOrDefault(p =>
			p.Birthday != null &&
			p.Birthday.Day == birthday.Day &&
			p.Birthday.Month == birthday.Month &&
			p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));

		if (sameBirthday != null)
		{
			_foundContactsWithSameBirthday = true;
			return false;
		}

		int nextId = _persons.Count > 0 ? _persons.Max(p => p.Id) + 1 : 1;
		var person = new Person
		{
			Id = nextId,
			FirstName = firstName,
			LastName = lastName,
			Birthday = new Birthday
			{
				Day = birthday.Day,
				Month = birthday.Month,
				Year = birthday.Year
			},
			ContactId = contactId,
			Source = PersonSource.Contacts
		};

		_persons.Add(person);
		BirthdayService.Add(person);
		return true;
	}

	/// <summary>
	/// Aim: Imports a birthday from the birthday calendar into the persons list and saves to prefs
	/// Params: firstName - First name, lastName - Last name, birthday - Birthday date
	/// Return: True if added, false if duplicate
	/// </summary>
	public bool ImportCalendarBirthday(string firstName, string lastName, DateTime birthday)
	{
		if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
			return false;

		var sameBirthday = _persons.FirstOrDefault(p =>
			p.Birthday != null &&
			p.Birthday.Day == birthday.Day &&
			p.Birthday.Month == birthday.Month &&
			p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));

		if (sameBirthday != null)
			return false;

		int nextId = _persons.Count > 0 ? _persons.Max(p => p.Id) + 1 : 1;
		var person = new Person
		{
			Id = nextId,
			FirstName = firstName,
			LastName = lastName,
			Birthday = new Birthday
			{
				Day = birthday.Day,
				Month = birthday.Month,
				Year = birthday.Year
			},
			Source = PersonSource.BirthdayCalendar
		};

		_persons.Add(person);
		BirthdayService.Add(person);
		return true;
	}

	private async void ReadBirthdaysFromBirthdayCalenderIfAllowed()
	{
		if (_settings.ContactsMode != ContactsMode.BirthdayCalendar)
			return;

		try
		{
			ContactBirthdayService service = new();
			List<Person> persons = await service.GetBirthdayCalendarEventsAsync();

			foreach (Person person in persons)
			{
				if (person.Birthday == null)
					continue;

				DateTime birthdayDate = BirthdayHelper.ConvertFromBirthdayToDateTime(person.Birthday);
				ImportCalendarBirthday(person.FirstName, person.LastName, birthdayDate);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading birthday calendar: {ex.Message}");
		}
	}
    #endregion

    #region Form Update Methods
    private async void OnNewBirthdayClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateEditBirthdayPage));
    }

    private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
        {
            await Shell.Current.GoToAsync($"{nameof(CreateEditBirthdayPage)}?Id={vm.Id}");
        }

        if (sender is CollectionView cv)
        {
            cv.SelectedItem = null;
        }
    }

	private void UpdateBirthdayListsOnTheForm()
	{
		if (_persons.Count == 0)
			return;

		var today = DateTime.Today;
		var nameDirection = _settings.PersonNameDirection;

		var upcoming = _persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysUntil = BirthdayHelper.GetDaysUntilBirthday(p.Birthday!, today) })
			.Where(x => x.DaysUntil >= 0)
			.OrderBy(x => x.DaysUntil)
			.Take(_settings.ShowUpcomingBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		var past = _persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysSince = BirthdayHelper.GetDaysSinceBirthday(p.Birthday!, today) })
			.Where(x => x.DaysSince > 0 && x.DaysSince <= 30)
			.OrderBy(x => x.DaysSince)
			.Take(_settings.ShowPastBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		UpcomingBirthdays = upcoming;
		MissedBirthdays = past;

		OnPropertyChanged(nameof(UpcomingBirthdays));
		OnPropertyChanged(nameof(MissedBirthdays));
	}
    #endregion
}

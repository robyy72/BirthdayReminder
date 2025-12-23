namespace Mobile;

public partial class MainPage : ContentPage
{
    #region Properties
    public List<PersonViewModel> UpcomingBirthdays { get; set; } = [];
	public List<PersonViewModel> MissedBirthdays { get; set; } = [];
    #endregion

    #region Private Class Variables
	private bool _foundContactsWithSameBirthday = false;
    private List<Person> _persons = [];
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
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, App.Account.ShowUpcomingBirthdays);
		MissedBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, App.Account.ShowPastBirthdays);
		if (App.NeedsReloadBirthdays)
		{
			LoadBirthdaysFromPrefs();
			App.NeedsReloadBirthdays = false;
		}
		UpdateBirthdayListsOnTheForm();
	}

	private void Init()
	{
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, App.Account.ShowUpcomingBirthdays);
		MissedBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, App.Account.ShowPastBirthdays);
		CheckRightsAndUpdateAccount();
		LoadBirthdaysFromPrefs();
		ReadContactsIfAllowed();
	}
    #endregion

    #region Init Methods
    private async void CheckRightsAndUpdateAccount()
    {
        if (App.Account.ContactsReadMode == ContactsReadMode.None || App.Account.ContactsReadMode == ContactsReadMode.NotSet)
            return;

        bool hasContactsRead = await DeviceService.CheckContactsReadPermissionAsync();
        if (!hasContactsRead)
        {
            App.Account.ContactsReadMode = ContactsReadMode.None;
            AccountService.Save();
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
		if (App.Account.ContactsReadMode != ContactsReadMode.ReadNamesWithBirthday &&
			App.Account.ContactsReadMode != ContactsReadMode.ReadAllNames)
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

    #endregion

    #region Form Update Methods
    private async void OnNewBirthdayClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateEditBirthdayPage_1));
    }

    private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
        {
            await Shell.Current.GoToAsync($"{nameof(CreateEditBirthdayPage_1)}?Id={vm.Id}");
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
		var nameDirection = App.Account.PersonNameDirection;

		var upcoming = _persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysUntil = BirthdayHelper.GetDaysUntilBirthday(p.Birthday!, today) })
			.Where(x => x.DaysUntil >= 0)
			.OrderBy(x => x.DaysUntil)
			.Take(App.Account.ShowUpcomingBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		var past = _persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysSince = BirthdayHelper.GetDaysSinceBirthday(p.Birthday!, today) })
			.Where(x => x.DaysSince > 0 && x.DaysSince <= 30)
			.OrderBy(x => x.DaysSince)
			.Take(App.Account.ShowPastBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		UpcomingBirthdays = upcoming;
		MissedBirthdays = past;

		OnPropertyChanged(nameof(UpcomingBirthdays));
		OnPropertyChanged(nameof(MissedBirthdays));
	}
    #endregion
}

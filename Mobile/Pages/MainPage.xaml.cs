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
			bool onlyWithBirthday = App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday;
			List<Person> persons = await service.GetContactsAsync(onlyWithBirthday);

			foreach (Person person in persons)
			{
				ImportContact(person);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error reading contacts: {ex.Message}");
		}
	}

	/// <summary>
	/// Aim: Imports a contact into the persons list and saves to prefs
	/// Params: person - The contact to import (may or may not have a birthday)
	/// Return: True if added, false if duplicate
	/// </summary>
	private bool ImportContact(Person person)
	{
		if (string.IsNullOrWhiteSpace(person.FirstName) && string.IsNullOrWhiteSpace(person.LastName))
			return false;

		// Check if contact already exists by ContactId
		if (!string.IsNullOrEmpty(person.ContactId))
		{
			var existingByContactId = _persons.FirstOrDefault(p => p.ContactId == person.ContactId);
			if (existingByContactId != null)
				return false;

			// Also check in App.Persons
			var existingInApp = App.Persons.FirstOrDefault(p => p.ContactId == person.ContactId);
			if (existingInApp != null)
				return false;
		}

		// Check for duplicate by name and birthday (if birthday exists)
		if (person.Birthday != null)
		{
			var sameBirthday = _persons.FirstOrDefault(p =>
				p.Birthday != null &&
				p.Birthday.Day == person.Birthday.Day &&
				p.Birthday.Month == person.Birthday.Month &&
				p.FirstName.Equals(person.FirstName, StringComparison.OrdinalIgnoreCase) &&
				p.LastName.Equals(person.LastName, StringComparison.OrdinalIgnoreCase));

			if (sameBirthday != null)
			{
				_foundContactsWithSameBirthday = true;
				return false;
			}
		}

		int nextId = Math.Max(
			_persons.Count > 0 ? _persons.Max(p => p.Id) + 1 : 1,
			App.Persons.Count > 0 ? App.Persons.Max(p => p.Id) + 1 : 1
		);

		person.Id = nextId;
		person.Source = PersonSource.Contacts;

		_persons.Add(person);
		App.Persons.Add(person);

		// Only save to BirthdayService if contact has a birthday
		if (person.Birthday != null)
		{
			BirthdayService.Add(person);
		}

		return true;
	}

    #endregion

    #region Form Update Methods
    private async void OnNewBirthdayClicked(object? sender, EventArgs e)
    {
        // If contacts are not used (None), go directly to CreateBirthday
        // Otherwise show search page to avoid duplicates
        if (App.Account.ContactsReadMode == ContactsReadMode.None)
        {
            await NavigationService.NavigateTo<CreateBirthdayPage>();
        }
        else
        {
            await NavigationService.NavigateTo<SearchPersonPage>();
        }
    }

    private async void OnAllBirthdaysClicked(object? sender, EventArgs e)
    {
        await NavigationService.NavigateTo<AllBirthdaysPage>();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await NavigationService.NavigateTo<AccountPage>();
    }

    private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
        {
            await NavigationService.NavigateTo<DetailBirthdayPage>(vm.Id);
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

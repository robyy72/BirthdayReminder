namespace Mobile;

public partial class MainPage : ContentPage
{
    #region Properties
    public List<PersonViewModel> UpcomingBirthdays { get; set; } = [];
	public List<PersonViewModel> PastBirthdays { get; set; } = [];
    #endregion

    #region Constructor and Start Methods
    public MainPage()
	{
		InitializeComponent();
		BindingContext = this;
		InitContextMenu();
		Init();
	}

	private void InitContextMenu()
	{
		TheContextMenu.AddMenuItem(MobileLanguages.Resources.Page_Main_NewBirthday, OnNewBirthdayFromMenu);
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		App.BackwardPageType = typeof(MainPage);
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, App.Account.ShowUpcomingBirthdays);
		PastBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, App.Account.ShowPastBirthdays);
		UpdateBirthdayListsOnTheForm();
	}

	private async void Init()
	{
		AccountService.CheckRightsAndUpdateAccount();
		if (App.NeedsReadContacts)
		{
			ContactsService.ReadContactsIfAllowed();
			if (App.Contacts.Count > 0)
			{
                await App.NavigateToAsync<SyncContactsToPersonsPage>();
            }
        }
	}
    #endregion

    #region Form Update Methods
    private async void OnNewBirthdayClicked(object? sender, EventArgs e)
    {
        CloseFlyout();

        // If contacts are not used (None), go directly to CreateBirthday
        // Otherwise show search page to avoid duplicates
        if (App.Account.ContactsReadMode == ContactsReadMode.None)
        {
            await App.NavigateToAsync<CreateBirthdayPage>();
        }
        else
        {
            await App.NavigateToAsync<SearchPersonPage>();
        }
    }

    private async void OnAllBirthdaysClicked(object? sender, EventArgs e)
    {
        CloseFlyout();
        await App.NavigateToAsync<AllBirthdaysPage>();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        CloseFlyout();
        await App.NavigateToAsync<AccountPage>();
    }

    private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
    {
        CloseFlyout();
        if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
        {
            await App.NavigateToAsync<DetailBirthdayPage>(vm.Id);
        }

        if (sender is CollectionView cv)
        {
            cv.SelectedItem = null;
        }
    }

	private void UpdateBirthdayListsOnTheForm()
	{
		if (App.Persons.Count == 0)
			return;

		var today = DateTime.Today;
		var nameDirection = App.Account.PersonNameDirection;

		var upcoming = App.Persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysUntil = BirthdayHelper.GetDaysUntilBirthday(p.Birthday!, today) })
			.Where(x => x.DaysUntil >= 0)
			.OrderBy(x => x.DaysUntil)
			.Take(App.Account.ShowUpcomingBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		var past = App.Persons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysSince = BirthdayHelper.GetDaysSinceBirthday(p.Birthday!, today) })
			.Where(x => x.DaysSince > 0 && x.DaysSince <= 30)
			.OrderBy(x => x.DaysSince)
			.Take(App.Account.ShowPastBirthdays)
			.Select(x => new PersonViewModel(x.Person, nameDirection))
			.ToList();

		UpcomingBirthdays = upcoming;
		PastBirthdays = past;

		OnPropertyChanged(nameof(UpcomingBirthdays));
		OnPropertyChanged(nameof(PastBirthdays));
	}
    #endregion

    #region Private Methods
    private void CloseFlyout()
    {
        App.CloseFlyout();
    }

    private async void OnNewBirthdayFromMenu()
    {
        // If contacts are not used (None), go directly to CreateBirthday
        // Otherwise show search page to avoid duplicates
        if (App.Account.ContactsReadMode == ContactsReadMode.None)
        {
            await App.NavigateToAsync<CreateBirthdayPage>();
        }
        else
        {
            await App.NavigateToAsync<SearchPersonPage>();
        }
    }
    #endregion
}

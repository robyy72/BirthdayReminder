using Common;

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
		AccountService.CheckRightsAndUpdateAccount();
		LoadContactsAfterWizardIfNeeded();
		PermissionService.ContactsPermissionChanged += OnContactsPermissionChanged;
	}

	private void InitContextMenu()
	{
		TheContextMenu.AddMenuItem(MobileLanguages.Resources.Page_Main_NewBirthday, OnNewBirthdayFromMenu);
		TheContextMenu.AddMenuItem(MobileLanguages.Resources.Support_ReportBug, OnReportBugFromMenu);
		TheContextMenu.AddMenuItem(MobileLanguages.Resources.Support_FeatureRequest, OnFeatureRequestFromMenu);
		TheContextMenu.AddMenuItem(MobileLanguages.Resources.Support_Feedback, OnFeedbackFromMenu);
	}

	/// <summary>
	/// Aim: Load contacts after wizard completes (first run only).
	/// </summary>
	private async void LoadContactsAfterWizardIfNeeded()
	{
		// Only needed when coming from wizard (contacts not loaded yet but permission granted)
		if (App.Contacts.Count > 0 || !AccountService.UseContacts())
			return;

		await ContactsService.ReadContactsIfAllowedAsync();

		if (App.Contacts.Count > 0 && App.NeedsSyncContacts)
		{
			await App.NavigateToAsync<SyncContactsToPersonsPage>();
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		App.BackwardPageType = typeof(MainPage);
		UpcomingBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_UpcomingTitle, App.Account.ShowUpcomingBirthdays);
		PastBirthdaysLabel.Text = string.Format(MobileLanguages.Resources.Page_Main_MissedTitle, App.Account.ShowPastBirthdays);
		UpdateBirthdayListsOnTheForm();
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
            await App.NavigateToAsync<CreatePersonPage>();
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
            await App.NavigateToAsync<DetailPersonPage>(vm.Id);
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
    /// <summary>
    /// Aim: Handle contacts permission change event.
    /// Params: status (AppPermissionStatus) - The new permission status.
    /// </summary>
    private async void OnContactsPermissionChanged(AppPermissionStatus status)
    {
        if (status == AppPermissionStatus.Granted)
        {
            await ContactsService.ReadContactsIfAllowedAsync();
            UpdateBirthdayListsOnTheForm();
        }
    }

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
            await App.NavigateToAsync<CreatePersonPage>();
        }
        else
        {
            await App.NavigateToAsync<SearchPersonPage>();
        }
    }

    private async void OnReportBugFromMenu()
    {
        await NavigateToSupportAsync(TicketType.Error);
    }

    private async void OnFeatureRequestFromMenu()
    {
        await NavigateToSupportAsync(TicketType.FeatureRequest);
    }

    private async void OnFeedbackFromMenu()
    {
        await NavigateToSupportAsync(TicketType.CustomerFeedback);
    }

    /// <summary>
    /// Aim: Navigate to ticket page - if entries exist go to list, otherwise create new.
    /// Params: ticketType (TicketType) - The type of ticket.
    /// </summary>
    private async Task NavigateToSupportAsync(TicketType ticketType)
    {
        var entries = TicketService.GetByType(ticketType);
        if (entries.Count > 0)
        {
            await App.NavigateToAsync<TicketListPage>(ticketType);
        }
        else
        {
            await App.NavigateToAsync<CreateTicketPage>(ticketType);
        }
    }
    #endregion
}

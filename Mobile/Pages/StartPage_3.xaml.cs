namespace Mobile;

public partial class StartPage_3 : ContentPage
{
	private bool _isLoading = true;

	public StartPage_3()
	{
		InitializeComponent();
		LoadAccount();
		_isLoading = false;
	}

	private void LoadAccount()
	{
		if (App.Account.ContactsMode == ContactsMode.BirthdayCalendar)
			RadioContactsBirthdayCalendar.IsChecked = true;
		else
			RadioContactsRead.IsChecked = true;
	}

	private async void OnContactsRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (!e.Value || _isLoading)
			return;

		if (sender == RadioContactsRead)
		{
			bool granted = await DeviceService.RequestContactsReadPermissionAsync();
			if (!granted)
			{
				RadioContactsBirthdayCalendar.IsChecked = true;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_ContactsRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
		else if (sender == RadioContactsBirthdayCalendar)
		{
			bool granted = await DeviceService.RequestCalendarReadPermissionAsync();
			if (!granted)
			{
				RadioContactsRead.IsChecked = true;
				await DisplayAlert(
					MobileLanguages.Resources.Permission_Required,
					MobileLanguages.Resources.Permission_CalendarRead_Denied,
					MobileLanguages.Resources.General_Button_OK);
			}
		}
	}

	private async void OnHelpContactsTapped(object? sender, EventArgs e)
	{
		var helpPage = new HelpPage { Topic = HelpTopic.UseContacts.ToString() };
		await Navigation.PushModalAsync(helpPage);
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_2();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (RadioContactsBirthdayCalendar.IsChecked)
			App.Account.ContactsMode = ContactsMode.BirthdayCalendar;
		else
			App.Account.ContactsMode = ContactsMode.ReadFromContacts;

		AccountService.Save();

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_4();
		}
	}
}

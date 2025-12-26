namespace Mobile;

public partial class StartPage_5 : ContentPage
{
	public StartPage_5()
	{
		InitializeComponent();
		LoadAccount();
	}

	private void LoadAccount()
	{
		switch (App.Account.ReminderCount)
		{
			case ReminderCount.NoReminder:
				RadioNoReminder.IsChecked = true;
				break;
			case ReminderCount.OneReminder:
				RadioOneReminder.IsChecked = true;
				break;
			case ReminderCount.TwoReminders:
				RadioTwoReminders.IsChecked = true;
				break;
			case ReminderCount.ThreeReminders:
				RadioThreeReminders.IsChecked = true;
				break;
			default:
				RadioNoReminder.IsChecked = true;
				break;
		}
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (AccountService.UseContacts())
		{
			App.BackwardPage = new StartPage_3();
			App.ForwardPage = new StartPage_5();
			App.SetRootPage(new RequestPermissionPage_1(PermissionType.Contacts));
		}
		else
		{
			App.SetRootPage(new StartPage_2());
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (RadioNoReminder.IsChecked)
			App.Account.ReminderCount = ReminderCount.NoReminder;
		else if (RadioOneReminder.IsChecked)
			App.Account.ReminderCount = ReminderCount.OneReminder;
		else if (RadioTwoReminders.IsChecked)
			App.Account.ReminderCount = ReminderCount.TwoReminders;
		else if (RadioThreeReminders.IsChecked)
			App.Account.ReminderCount = ReminderCount.ThreeReminders;

		AccountService.Save();

		if (App.Account.ReminderCount == ReminderCount.NoReminder)
			App.SetRootPage(new StartPage_9());
		else
			App.SetRootPage(new StartPage_6());
	}
}

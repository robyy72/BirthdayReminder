namespace Mobile;

public partial class StartPage_4 : ContentPage
{
	public StartPage_4()
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
		if (Application.Current?.Windows.Count > 0)
		{
			if (App.UseContacts)
				Application.Current.Windows[0].Page = new StartPage_3();
			else
				Application.Current.Windows[0].Page = new StartPage_2();
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

		if (Application.Current?.Windows.Count > 0)
		{
			if (App.Account.ReminderCount == ReminderCount.NoReminder)
				Application.Current.Windows[0].Page = new StartPage_8();
			else
				Application.Current.Windows[0].Page = new StartPage_5();
		}
	}
}

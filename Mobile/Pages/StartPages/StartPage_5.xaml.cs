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
		switch (App.Account.RequestedReminderTemplatesCount)
		{
			case 0:
				RadioNoReminder.IsChecked = true;
				break;
			case 1:
				RadioOneReminder.IsChecked = true;
				break;
			case 2:
				RadioTwoReminders.IsChecked = true;
				break;
			case 3:
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
			App.BackwardPageType = typeof(StartPage_3);
			App.ForwardPageType = typeof(StartPage_5);
			App.SetRootPage(new RequestPermissionPage_1(PermissionType.Contacts));
		}
		else
		{
			App.SetRootPage(new StartPage_2());
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		int newCount = 0;
		if (RadioOneReminder.IsChecked)
			newCount = 1;
		else if (RadioTwoReminders.IsChecked)
			newCount = 2;
		else if (RadioThreeReminders.IsChecked)
			newCount = 3;

		App.Account.RequestedReminderTemplatesCount = newCount;

		// Clear reminder templates beyond the new count
		for (int i = newCount; i < App.Account.ReminderTemplates.Length; i++)
		{
			App.Account.ReminderTemplates[i] = null;
		}

		AccountService.Save();

		if (newCount == 0)
			App.SetRootPage(new StartPage_9());
		else
			App.SetRootPage(new ReminderPage(0));
	}
}

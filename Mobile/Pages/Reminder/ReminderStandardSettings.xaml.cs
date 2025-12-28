namespace Mobile;

public partial class ReminderStandardSettings : ContentPage
{
	#region Constructor
	public ReminderStandardSettings()
	{
		InitializeComponent();
		LoadSettings();
	}
	#endregion

	#region Load
	private void LoadSettings()
	{
		var account = App.Account;

		switch (account.RequestedReminderTemplatesCount)
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
		}

		if (App.Account.ReminderTemplates[0] != null)
		{
			Reminder1Days.Text = App.Account.ReminderTemplates[0].DaysBefore.ToString();
		}

		if (App.Account.ReminderTemplates[1] != null)
		{
			Reminder2Days.Text = App.Account.ReminderTemplates[1].DaysBefore.ToString();
		}

		if (App.Account.ReminderTemplates[2] != null)
		{
			Reminder3Days.Text = App.Account.ReminderTemplates[2].DaysBefore.ToString();
		}

		UpdateSectionsVisibility();
	}
	#endregion

	#region Update Visibility
	private void UpdateSectionsVisibility()
	{
		var count = GetSelectedReminderCount();

		Reminder1Section.IsVisible = count >= 1;
		Reminder2Section.IsVisible = count >= 2;
		Reminder3Section.IsVisible = count >= 3;
	}

	private int GetSelectedReminderCount()
	{
		if (RadioThreeReminders.IsChecked)
			return 3;
		if (RadioTwoReminders.IsChecked)
			return 2;
		if (RadioOneReminder.IsChecked)
			return 1;
		return 0;
	}
	#endregion

	#region Event Handlers
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var account = App.Account;
		account.RequestedReminderTemplatesCount = GetSelectedReminderCount();

		if (int.TryParse(Reminder1Days.Text, out int days1) && App.Account.ReminderTemplates[0] != null)
		{
			App.Account.ReminderTemplates[0].DaysBefore = days1;
		}

		if (int.TryParse(Reminder2Days.Text, out int days2) && App.Account.ReminderTemplates[1] != null)
		{
			App.Account.ReminderTemplates[1].DaysBefore = days2;
		}

		if (int.TryParse(Reminder3Days.Text, out int days3) && App.Account.ReminderTemplates[2] != null)
		{
			App.Account.ReminderTemplates[2].DaysBefore = days3;
		}

		AccountService.Save();

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await App.GoBackAsync();
	}
	#endregion
}

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

		switch (account.ReminderCount)
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
		}

		if (App.Reminder_1_Template != null)
		{
			Reminder1Days.Text = App.Reminder_1_Template.Days.ToString();
		}

		if (App.Reminder_2_Template != null)
		{
			Reminder2Days.Text = App.Reminder_2_Template.Days.ToString();
		}

		if (App.Reminder_3_Template != null)
		{
			Reminder3Days.Text = App.Reminder_3_Template.Days.ToString();
		}

		UpdateSectionsVisibility();
	}
	#endregion

	#region Update Visibility
	private void UpdateSectionsVisibility()
	{
		var count = GetSelectedReminderCount();

		Reminder1Section.IsVisible = count >= ReminderCount.OneReminder;
		Reminder2Section.IsVisible = count >= ReminderCount.TwoReminders;
		Reminder3Section.IsVisible = count >= ReminderCount.ThreeReminders;
	}

	private ReminderCount GetSelectedReminderCount()
	{
		if (RadioThreeReminders.IsChecked)
			return ReminderCount.ThreeReminders;
		if (RadioTwoReminders.IsChecked)
			return ReminderCount.TwoReminders;
		if (RadioOneReminder.IsChecked)
			return ReminderCount.OneReminder;
		return ReminderCount.NoReminder;
	}
	#endregion

	#region Event Handlers
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var account = App.Account;
		account.ReminderCount = GetSelectedReminderCount();

		if (int.TryParse(Reminder1Days.Text, out int days1) && App.Reminder_1_Template != null)
		{
			App.Reminder_1_Template.Days = days1;
		}

		if (int.TryParse(Reminder2Days.Text, out int days2) && App.Reminder_2_Template != null)
		{
			App.Reminder_2_Template.Days = days2;
		}

		if (int.TryParse(Reminder3Days.Text, out int days3) && App.Reminder_3_Template != null)
		{
			App.Reminder_3_Template.Days = days3;
		}

		AccountService.Save();

		await DisplayAlert(
			MobileLanguages.Resources.Settings_Saved_Title,
			MobileLanguages.Resources.Settings_Saved_Message,
			MobileLanguages.Resources.General_Button_OK);
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await NavigationService.GoBack();
	}
	#endregion
}

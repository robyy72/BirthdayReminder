namespace Mobile;

public partial class StartPage_6 : ContentPage
{
	public StartPage_6()
	{
		InitializeComponent();
		LoadReminder();
	}

	private void LoadReminder()
	{
		var reminder = App.Persons.FirstOrDefault()?.Reminder_1;
		if (reminder != null)
		{
			DaysEntry.Text = reminder.Days.ToString();
			EmailSwitch.IsToggled = reminder.EmailEnabled;
			SmsSwitch.IsToggled = reminder.SmsEnabled;
			LockScreenSwitch.IsToggled = reminder.LockScreenEnabled;
			WhatsAppSwitch.IsToggled = reminder.WhatsAppEnabled;
			SignalSwitch.IsToggled = reminder.SignalEnabled;
		}
	}

	private Reminder CreateReminder()
	{
		int.TryParse(DaysEntry.Text, out int days);

		var reminder = new Reminder
		{
			Days = days,
			EmailEnabled = EmailSwitch.IsToggled,
			SmsEnabled = SmsSwitch.IsToggled,
			LockScreenEnabled = LockScreenSwitch.IsToggled,
			WhatsAppEnabled = WhatsAppSwitch.IsToggled,
			SignalEnabled = SignalSwitch.IsToggled
		};

		return reminder;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(new StartPage_5());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		// Store Reminder_1 template in a static variable for later use
		App.Reminder_1_Template = CreateReminder();

		if (App.Account.ReminderCount >= ReminderCount.TwoReminders)
			App.SetRootPage(new StartPage_7());
		else
			App.SetRootPage(new StartPage_9());
	}
}

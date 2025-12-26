namespace Mobile;

public partial class StartPage_7 : ContentPage
{
	public StartPage_7()
	{
		InitializeComponent();
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
		App.SetRootPage(new StartPage_6());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.Reminder_2_Template = CreateReminder();

		if (App.Account.ReminderCount == ReminderCount.ThreeReminders)
			App.SetRootPage(new StartPage_8());
		else
			App.SetRootPage(new StartPage_9());
	}
}

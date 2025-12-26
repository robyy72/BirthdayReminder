namespace Mobile;

public partial class StartPage_8 : ContentPage
{
	public StartPage_8()
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
		App.SetRootPage(new StartPage_7());
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.Reminder_3_Template = CreateReminder();
		App.SetRootPage(new StartPage_9());
	}
}

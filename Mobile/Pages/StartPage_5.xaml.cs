namespace Mobile;

public partial class StartPage_5 : ContentPage
{
	public StartPage_5()
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
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_4();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		// Store Reminder_1 template in a static variable for later use
		App.Reminder_1_Template = CreateReminder();

		if (Application.Current?.Windows.Count > 0)
		{
			if (App.Account.ReminderCount >= ReminderCount.TwoReminders)
				Application.Current.Windows[0].Page = new StartPage_6();
			else
				Application.Current.Windows[0].Page = new StartPage_8();
		}
	}
}

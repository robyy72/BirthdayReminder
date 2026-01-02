#region Usings
using Common;
#endregion

namespace Mobile;

public partial class ReminderSelectPage : ContentPage
{
	#region Constructor
	public ReminderSelectPage()
	{
		InitializeComponent();
	}
	#endregion

	#region Lifecycle
	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadReminderCards();
	}
	#endregion

	#region Private Methods
	private void LoadReminderCards()
	{
		var reminderCount = App.Account.RequestedReminderTemplatesCount;

		// Show/hide cards based on reminder count
		Reminder1Card.IsVisible = reminderCount >= 1;
		Reminder2Card.IsVisible = reminderCount >= 2;
		Reminder3Card.IsVisible = reminderCount >= 3;

		// Load reminder details
		if (Reminder1Card.IsVisible)
			LoadReminderCard(0, App.Account.ReminderTemplates[0], Reminder1DaysLabel, Reminder1MethodsLabel);

		if (Reminder2Card.IsVisible)
			LoadReminderCard(1, App.Account.ReminderTemplates[1], Reminder2DaysLabel, Reminder2MethodsLabel);

		if (Reminder3Card.IsVisible)
			LoadReminderCard(2, App.Account.ReminderTemplates[2], Reminder3DaysLabel, Reminder3MethodsLabel);
	}

	private void LoadReminderCard(int index, Reminder? reminder, Label daysLabel, Label methodsLabel)
	{
		if (reminder == null)
		{
			daysLabel.Text = MobileLanguages.Resources.ReminderSettings_NotConfigured;
			methodsLabel.Text = "";
			return;
		}

		// Days before
		daysLabel.Text = string.Format(MobileLanguages.Resources.ReminderSettings_DaysBefore, reminder.DaysBefore);

		// Active methods
		var activeMethods = new List<string>();

		foreach (var method in reminder.LocalMethods.Where(m => m.Enabled))
		{
			var methodName = method.Type switch
			{
				LocalMethodType.Notification => MobileLanguages.Resources.ReminderMethod_Notification,
				LocalMethodType.Alarm => MobileLanguages.Resources.ReminderMethod_Alarm,
				_ => method.Type.ToString()
			};
			activeMethods.Add(methodName);
		}

		foreach (var method in reminder.ExternalMethods.Where(m => m.Enabled))
		{
			var methodName = method.Type switch
			{
				ExternalMethodType.Email => MobileLanguages.Resources.ReminderMethod_Email,
				ExternalMethodType.Sms => MobileLanguages.Resources.ReminderMethod_Sms,
				ExternalMethodType.WhatsApp => MobileLanguages.Resources.ReminderMethod_WhatsApp,
				ExternalMethodType.Signal => MobileLanguages.Resources.ReminderMethod_Signal,
				_ => method.Type.ToString()
			};
			activeMethods.Add(methodName);
		}

		methodsLabel.Text = activeMethods.Count > 0
			? string.Join(", ", activeMethods)
			: MobileLanguages.Resources.ReminderSettings_NoMethods;
	}
	#endregion

	#region Event Handlers
	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await App.GoBackAsync();
	}

	private async void OnReminder1Tapped(object? sender, TappedEventArgs e)
	{
		App.ForwardPageType = typeof(ReminderSelectPage);
		await App.NavigateToAsync<ReminderPage>(0);
	}

	private async void OnReminder2Tapped(object? sender, TappedEventArgs e)
	{
		App.ForwardPageType = typeof(ReminderSelectPage);
		await App.NavigateToAsync<ReminderPage>(1);
	}

	private async void OnReminder3Tapped(object? sender, TappedEventArgs e)
	{
		App.ForwardPageType = typeof(ReminderSelectPage);
		await App.NavigateToAsync<ReminderPage>(2);
	}
	#endregion
}

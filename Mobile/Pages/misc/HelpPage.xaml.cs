#region Usings
#endregion

namespace Mobile;

public partial class HelpPage : ContentPage
{
	private HelpTopic _topic;

	public string Topic
	{
		get => _topic.ToString();
		set
		{
			if (Enum.TryParse<HelpTopic>(value, out var topic))
			{
				_topic = topic;
			}
		}
	}

	public HelpPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadContent();
	}

	private void LoadContent()
	{
		switch (_topic)
		{
			case HelpTopic.UseContacts:
				HeaderLabel.Text = MobileLanguages.Resources.Help_UseContacts_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_UseContacts_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_UseContacts_Content_2;
				break;

			case HelpTopic.BirthdayWithoutYear:
				HeaderLabel.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Content_2;
				break;

			case HelpTopic.FreeReminders:
				HeaderLabel.Text = MobileLanguages.Resources.Help_FreeReminders_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_FreeReminders_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_FreeReminders_Content_2;
				break;

			case HelpTopic.ReminderNotification:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderNotification_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderNotification_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderNotification_Content_2;
				ShowTimezoneCard();
				break;

			case HelpTopic.ReminderAlarm:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderAlarm_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderAlarm_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderAlarm_Content_2;
				ShowTimezoneCard();
				break;

			case HelpTopic.ReminderEmail:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderEmail_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderEmail_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderEmail_Content_2;
				ShowTimezoneCard();
				break;

			case HelpTopic.ReminderSignal:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderSignal_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderSignal_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderSignal_Content_2;
				ShowTimezoneCard();
				break;

			case HelpTopic.ReminderSms:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderSms_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderSms_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderSms_Content_2;
				ShowTimezoneCard();
				break;

			case HelpTopic.ReminderWhatsApp:
				HeaderLabel.Text = MobileLanguages.Resources.Help_ReminderWhatsApp_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_ReminderWhatsApp_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_ReminderWhatsApp_Content_2;
				ShowTimezoneCard();
				break;

			default:
				HeaderLabel.Text = MobileLanguages.Resources.Page_Help_Title;
				ContentLabel_1.Text = "";
				ContentLabel_2.Text = "";
				break;
		}
	}

	private void ShowTimezoneCard()
	{
		var tzInfo = DeviceService.GetTimeZoneInfo();
		string timezoneNote = string.Format(
			MobileLanguages.Resources.Help_Timezone_Note,
			tzInfo.Abbreviation,
			tzInfo.FullName,
			tzInfo.Offset);
		TimezoneLabel.Text = timezoneNote;
		TimezoneCard.IsVisible = true;
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		if (Navigation.ModalStack.Count > 0)
		{
			await Navigation.PopModalAsync();
		}
		else
		{
			await App.GoBackAsync();
		}
	}

	private async void OnTimezoneInfoTapped(object? sender, EventArgs e)
	{
		var browserPage = new BrowserPage(
			MobileConstants.URL_TIMEZONE_INFO,
			MobileLanguages.Resources.Timezone_MoreInfo);
		await Navigation.PushModalAsync(browserPage);
	}
}

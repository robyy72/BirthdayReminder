#region Usings
#endregion

namespace Mobile;

[QueryProperty(nameof(Topic), "Topic")]
public partial class HelpPage : ContentPage
{
	private HelpTopic _topic;

	public string Topic
	{
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
				ContentLabel.Text = MobileLanguages.Resources.Help_UseContacts_Content;
				break;

			case HelpTopic.ReadFromBirthdayCalendar:
				HeaderLabel.Text = MobileLanguages.Resources.Help_BirthdayCalendar_Header;
				ContentLabel.Text = MobileLanguages.Resources.Help_BirthdayCalendar_Content;
				break;

			default:
				HeaderLabel.Text = MobileLanguages.Resources.Page_Help_Title;
				ContentLabel.Text = "";
				break;
		}
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}

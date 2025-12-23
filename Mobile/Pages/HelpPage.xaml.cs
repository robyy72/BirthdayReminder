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
				ContentLabel_1.Text = MobileLanguages.Resources.Help_UseContacts_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_UseContacts_Content_2;
				break;

			case HelpTopic.BirthdayWithoutYear:
				HeaderLabel.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Header;
				ContentLabel_1.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Content_1;
				ContentLabel_2.Text = MobileLanguages.Resources.Help_BirthdayWithoutYear_Content_2;
				break;

			default:
				HeaderLabel.Text = MobileLanguages.Resources.Page_Help_Title;
				ContentLabel_1.Text = "";
				ContentLabel_2.Text = "";
				break;
		}
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		if (Navigation.ModalStack.Count > 0)
		{
			await Navigation.PopModalAsync();
		}
		else
		{
			await Shell.Current.GoToAsync("..");
		}
	}
}

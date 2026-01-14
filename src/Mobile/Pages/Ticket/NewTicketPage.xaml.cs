#region Usings
using Common;
#endregion

namespace Mobile;

public partial class NewTicketPage : ContentPage
{
	#region Fields
	private readonly SupportType _ticketType;
	#endregion

	#region Constructor
	public NewTicketPage(SupportType ticketType)
	{
		InitializeComponent();
		_ticketType = ticketType;
		SetupHeader();
	}
	#endregion

	#region Private Methods
	private void SetupHeader()
	{
		TypeHeaderLabel.Text = _ticketType switch
		{
			SupportType.Bug => MobileLanguages.Resources.Ticket_Type_Bug,
			SupportType.FeatureRequest => MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			SupportType.Feedback => MobileLanguages.Resources.Ticket_Type_Feedback,
			_ => MobileLanguages.Resources.Ticket_Type_Feedback
		};
	}
	#endregion

	#region Event Handlers
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var title = TitleEntry.Text?.Trim() ?? string.Empty;
		var text = TextEditor.Text?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(title))
		{
			ErrorLabel.Text = MobileLanguages.Resources.Error_NameRequired;
			ErrorBorder.IsVisible = true;
			return;
		}

		ErrorBorder.IsVisible = false;

		var ticket = new Support
		{
			Type = (int)_ticketType,
			Title = title,
			Text = text
		};

		var uploaded = await TicketService.UploadTicketAsync(ticket);

		if (!uploaded)
		{
			await DisplayAlert(
				MobileLanguages.Resources.NoInternet_Title,
				MobileLanguages.Resources.Ticket_SavedOffline,
				MobileLanguages.Resources.General_Button_OK);
			await App.GoBackAsync();
			return;
		}

		await App.GoBackAsync();
		await App.NavigateToAsync<TicketDetailPage>(ticket.Id);
	}

	private void OnEntryFocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			outerBorder.Stroke = ResourceHelper.GetColor("Primary");
			outerBorder.StrokeThickness = 2;
		}
	}

	private void OnEntryUnfocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			outerBorder.Stroke = ResourceHelper.GetThemedColor("Gray300", "Gray700");
			outerBorder.StrokeThickness = 1;
		}
	}
	#endregion
}

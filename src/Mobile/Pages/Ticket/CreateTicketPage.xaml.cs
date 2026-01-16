#region Usings
using Common;
#endregion

namespace Mobile;

public partial class CreateTicketPage : ContentPage
{
	#region Fields
	private readonly TicketType _ticketType;
	#endregion

	#region Constructor
	public CreateTicketPage(TicketType ticketType)
	{
		InitializeComponent();
		_ticketType = ticketType;
		SetupUI();
	}
	#endregion

	#region Private Methods
	private void SetupUI()
	{
		TheHeader.Title = _ticketType switch
		{
			TicketType.Error => MobileLanguages.Resources.Ticket_Type_Bug,
			TicketType.FeatureRequest => MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			TicketType.CustomerFeedback => MobileLanguages.Resources.Ticket_Type_Feedback,
			_ => MobileLanguages.Resources.Ticket_Type_Feedback
		};

		(Text1Editor.Placeholder, Text2Editor.Placeholder) = _ticketType switch
		{
			TicketType.Error => (
				MobileLanguages.Resources.Ticket_Placeholder_Bug_Text_1,
				MobileLanguages.Resources.Ticket_Placeholder_Bug_Text_2),
			TicketType.FeatureRequest => (
				MobileLanguages.Resources.Ticket_Placeholder_Feature_Text_1,
				MobileLanguages.Resources.Ticket_Placeholder_Feature_Text_2),
			TicketType.CustomerFeedback => (
				MobileLanguages.Resources.Ticket_Placeholder_Feedback_Text_1,
				MobileLanguages.Resources.Ticket_Placeholder_Feedback_Text_2),
			_ => (
				MobileLanguages.Resources.Ticket_Placeholder_Feedback_Text_1,
				MobileLanguages.Resources.Ticket_Placeholder_Feedback_Text_2)
		};
	}
	#endregion

	#region Event Handlers
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var title = TitleEntry.Text?.Trim() ?? string.Empty;
		var text1 = Text1Editor.Text?.Trim() ?? string.Empty;
		var text2 = Text2Editor.Text?.Trim() ?? string.Empty;

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
			Text = text1,
			Text2 = text2
		};

		var uploaded = await TicketService.UploadTicketAsync(ticket);

		if (!uploaded)
		{
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

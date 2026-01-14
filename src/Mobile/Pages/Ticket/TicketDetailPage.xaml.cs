#region Usings
using Common;
#endregion

namespace Mobile;

public partial class TicketDetailPage : ContentPage
{
	#region Fields
	private readonly int _ticketId;
	private Support? _ticket;
	private bool _isAnswerAreaVisible = false;
	#endregion

	#region Constructor
	public TicketDetailPage(int ticketId)
	{
		InitializeComponent();
		_ticketId = ticketId;
	}
	#endregion

	#region Lifecycle
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadTicket();
	}
	#endregion

	#region Private Methods
	private async Task LoadTicket()
	{
		_ticket = TicketService.GetById(_ticketId);
		if (_ticket == null)
		{
			await App.GoBackAsync();
			return;
		}

		var typeText = ((TicketType)_ticket.Type) switch
		{
			TicketType.Error => MobileLanguages.Resources.Ticket_Type_Bug,
			TicketType.FeatureRequest => MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			TicketType.CustomerFeedback => MobileLanguages.Resources.Ticket_Type_Feedback,
			_ => MobileLanguages.Resources.Ticket_Type_Feedback
		};

		TypeHeaderLabel.Text = typeText;
		TitleLabel.Text = _ticket.Title;
		TextLabel.Text = _ticket.Text;

		LoadEntries();
	}

	private void LoadEntries()
	{
		EntriesContainer.Children.Clear();

		if (_ticket == null)
			return;

		var entries = TicketService.GetEntriesByTicketId(_ticketId);

		if (entries.Count > 0)
		{
			var headerLabel = new Label
			{
				Text = MobileLanguages.Resources.Ticket_Entries,
				Style = ResourceHelper.GetStyle("LabelSubHeader"),
				Margin = new Thickness(0, 10, 0, 5)
			};
			EntriesContainer.Children.Add(headerLabel);

			foreach (var entry in entries)
			{
				var entryCard = CreateEntryCard(entry);
				EntriesContainer.Children.Add(entryCard);
			}
		}
	}

	private Border CreateEntryCard(SupportEntry entry)
	{
		var card = new Border
		{
			Style = ResourceHelper.GetStyle("Tile"),
			Padding = new Thickness(15)
		};

		var stack = new VerticalStackLayout { Spacing = 5 };

		var textLabel = new Label
		{
			Text = entry.Text,
			Style = ResourceHelper.GetStyle("LabelInfoOnCard")
		};

		var dateLabel = new Label
		{
			Text = entry.CreatedAt.ToString("g"),
			Style = ResourceHelper.GetStyle("LabelDescriptionOnCard")
		};

		stack.Children.Add(textLabel);
		stack.Children.Add(dateLabel);

		card.Content = stack;

		return card;
	}

	/// <summary>
	/// Aim: Show answer input area with expanding animation.
	/// </summary>
	private async Task ShowAnswerAreaAsync()
	{
		AnswerInputBorder.IsVisible = true;
		AnswerInputBorder.Scale = 0.95;
		AnswerInputBorder.Opacity = 0;

		await Task.WhenAll(
			AnswerInputBorder.FadeTo(1, 250, Easing.CubicOut),
			AnswerInputBorder.ScaleTo(1, 250, Easing.CubicOut)
		);

		AnswerEditor.Focus();
		_isAnswerAreaVisible = true;
		AnswerButton.IsVisible = false;
	}

	/// <summary>
	/// Aim: Hide answer input area with collapsing animation.
	/// </summary>
	private async Task HideAnswerAreaAsync()
	{
		await Task.WhenAll(
			AnswerInputBorder.FadeTo(0, 200, Easing.CubicIn),
			AnswerInputBorder.ScaleTo(0.95, 200, Easing.CubicIn)
		);

		AnswerInputBorder.IsVisible = false;
		AnswerEditor.Text = string.Empty;
		_isAnswerAreaVisible = false;
		AnswerButton.IsVisible = true;
	}
	#endregion

	#region Event Handlers
	private async void OnAnswerClicked(object? sender, EventArgs e)
	{
		await ShowAnswerAreaAsync();
	}

	private async void OnSendAnswerClicked(object? sender, EventArgs e)
	{
		var text = AnswerEditor.Text?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(text))
			return;

		var entry = new SupportEntry
		{
			SupportId = _ticketId,
			Text = text
		};

		TicketService.AddEntry(entry);

		await HideAnswerAreaAsync();
		LoadEntries();
	}

	#endregion
}

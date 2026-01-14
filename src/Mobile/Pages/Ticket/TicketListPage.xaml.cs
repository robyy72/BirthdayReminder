#region Usings
using Common;
#endregion

namespace Mobile;

public partial class TicketListPage : ContentPage
{
	#region Fields
	private readonly TicketType _ticketType;
	private int _selectedFilterIndex = 0;
	private bool _isFirstLoad = true;
	#endregion

	#region Constructor
	public TicketListPage(TicketType ticketType)
	{
		InitializeComponent();
		_ticketType = ticketType;
		SetupFilter();
	}
	#endregion

	#region Lifecycle
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (_isFirstLoad)
		{
			_isFirstLoad = false;
			await LoadTicketsAsync();
		}
		else
		{
			LoadEntries();
		}
	}
	#endregion

	#region Private Methods
	private void SetupFilter()
	{
		var filterItems = new List<string>
		{
			MobileLanguages.Resources.Filter_All,
			MobileLanguages.Resources.Ticket_Type_Bug,
			MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			MobileLanguages.Resources.Ticket_Type_Feedback
		};

		FilterPicker.ItemsSource = filterItems;

		// Pre-select filter based on ticket type
		_selectedFilterIndex = _ticketType switch
		{
			TicketType.Error => 1,
			TicketType.FeatureRequest => 2,
			TicketType.CustomerFeedback => 3,
			_ => 0
		};
		FilterPicker.SelectedIndex = _selectedFilterIndex;
	}

	private async Task LoadTicketsAsync()
	{
		ShowLoading(true);

		var success = await TicketService.DownloadTicketsAsync();

		if (!success)
		{
			await DisplayAlert(
				MobileLanguages.Resources.Error_Header,
				MobileLanguages.Resources.Error_GenericMessage,
				MobileLanguages.Resources.General_Button_OK);
		}

		ShowLoading(false);
		LoadEntries();
	}

	private void ShowLoading(bool isLoading)
	{
		LoadingIndicator.IsRunning = isLoading;
		LoadingIndicator.IsVisible = isLoading;
		ContentScrollView.IsVisible = !isLoading;
	}

	private void LoadEntries()
	{
		EntriesContainer.Children.Clear();

		List<Support> entries;

		if (_selectedFilterIndex == 0)
		{
			entries = TicketService.GetAll();
		}
		else
		{
			var ticketType = _selectedFilterIndex switch
			{
				1 => TicketType.Error,
				2 => TicketType.FeatureRequest,
				3 => TicketType.CustomerFeedback,
				_ => TicketType.CustomerFeedback
			};
			entries = TicketService.GetByType(ticketType);
		}

		foreach (var entry in entries)
		{
			var card = CreateEntryCard(entry);
			EntriesContainer.Children.Add(card);
		}

		if (entries.Count == 0)
		{
			var emptyLabel = new Label
			{
				Text = MobileLanguages.Resources.EmptyView_NoTickets,
				Style = ResourceHelper.GetStyle("LabelInfoOnPage"),
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 20, 0, 0)
			};
			EntriesContainer.Children.Add(emptyLabel);
		}
	}

	private Border CreateEntryCard(Support entry)
	{
		var typeText = ((TicketType)entry.Type) switch
		{
			TicketType.Error => MobileLanguages.Resources.Ticket_Type_Bug,
			TicketType.FeatureRequest => MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			TicketType.CustomerFeedback => MobileLanguages.Resources.Ticket_Type_Feedback,
			_ => MobileLanguages.Resources.Ticket_Type_Feedback
		};

		var headerText = $"{typeText}: {entry.Title}";

		var card = new Border
		{
			Style = ResourceHelper.GetStyle("Card")
		};

		var stackLayout = new VerticalStackLayout { Spacing = 0 };

		// Header
		var headerBorder = new Border
		{
			Style = ResourceHelper.GetStyle("CardHeader")
		};
		var headerLabel = new Label
		{
			Text = headerText,
			Style = ResourceHelper.GetStyle("LabelCardHeader")
		};
		headerBorder.Content = headerLabel;

		// Content
		var contentStack = new VerticalStackLayout
		{
			Padding = new Thickness(20),
			Spacing = 5
		};
		var textLabel = new Label
		{
			Text = entry.Text,
			Style = ResourceHelper.GetStyle("LabelInfoOnCard"),
			MaxLines = 3,
			LineBreakMode = LineBreakMode.TailTruncation
		};
		contentStack.Children.Add(textLabel);

		stackLayout.Children.Add(headerBorder);
		stackLayout.Children.Add(contentStack);

		card.Content = stackLayout;

		// Tap gesture
		var tapGesture = new TapGestureRecognizer();
		tapGesture.Tapped += async (s, e) =>
		{
			await App.NavigateToAsync<TicketDetailPage>(entry.Id);
		};
		card.GestureRecognizers.Add(tapGesture);

		return card;
	}

	/// <summary>
	/// Aim: Get the current filter type for creating new tickets.
	/// Return: TicketType based on current filter selection.
	/// </summary>
	private TicketType GetCurrentFilterType()
	{
		var result = _selectedFilterIndex switch
		{
			1 => TicketType.Error,
			2 => TicketType.FeatureRequest,
			3 => TicketType.CustomerFeedback,
			_ => _ticketType
		};
		return result;
	}
	#endregion

	#region Event Handlers
	private void OnFilterChanged(object? sender, EventArgs e)
	{
		_selectedFilterIndex = FilterPicker.SelectedIndex;
		LoadEntries();
	}

	private async void OnAddClicked(object? sender, EventArgs e)
	{
		await App.NavigateToAsync<SelectTicketTypePage>();
	}
	#endregion
}

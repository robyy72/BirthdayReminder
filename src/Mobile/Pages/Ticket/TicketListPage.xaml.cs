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
	public TicketListPage() : this(TicketType.CustomerFeedback)
	{
		// Show all tickets
		_selectedFilterIndex = 0;
		FilterDropdown.SelectedIndex = 0;
	}

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
		var filteredBy = MobileLanguages.Resources.Filter_FilteredBy;
		var filterItems = new List<string>
		{
			MobileLanguages.Resources.Filter_AllTicketTypes,
			$"{filteredBy} {MobileLanguages.Resources.Ticket_Type_Bug}",
			$"{filteredBy} {MobileLanguages.Resources.Ticket_Type_FeatureRequest}",
			$"{filteredBy} {MobileLanguages.Resources.Ticket_Type_Feedback}"
		};

		FilterDropdown.ItemsSource = filterItems;

		// Pre-select filter based on ticket type
		_selectedFilterIndex = _ticketType switch
		{
			TicketType.Error => 1,
			TicketType.FeatureRequest => 2,
			TicketType.CustomerFeedback => 3,
			_ => 0
		};
		FilterDropdown.SelectedIndex = _selectedFilterIndex;
	}

	private async Task LoadTicketsAsync()
	{
		ShowLoading(true);

		var (success, hadNetwork) = await TicketService.DownloadTicketsAsync();

		ShowLoading(false);
		LoadEntries();

		if (!success)
		{
			if (!hadNetwork && !App.ToastNoInternetAlreadyShown)
			{
				// No internet - show info toast
				App.ToastNoInternetAlreadyShown = true;
				await DisplayAlert(
					MobileLanguages.Resources.Info_NoInternet_Title,
					MobileLanguages.Resources.Info_NoInternet_TicketDownload,
					MobileLanguages.Resources.General_Button_OK);
			}
			else if (hadNetwork)
			{
				// Had internet but API failed - show error page
				await App.NavigateToAsync<ErrorDisplayPage>(
					MobileLanguages.Resources.Error_ApiUnavailable);
			}
		}
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
			var createButton = new Button
			{
				Text = MobileLanguages.Resources.Button_CreateTicket,
				Style = ResourceHelper.GetStyle("ButtonPrimary"),
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 20, 0, 0)
			};
			createButton.Clicked += async (s, e) => await App.NavigateToAsync<SelectTicketTypePage>();
			EntriesContainer.Children.Add(createButton);
		}
	}

	private Frame CreateEntryCard(Support entry)
	{
		var typeText = ((TicketType)entry.Type) switch
		{
			TicketType.Error => MobileLanguages.Resources.Ticket_Type_Bug,
			TicketType.FeatureRequest => MobileLanguages.Resources.Ticket_Type_FeatureRequest,
			TicketType.CustomerFeedback => MobileLanguages.Resources.Ticket_Type_Feedback,
			_ => MobileLanguages.Resources.Ticket_Type_Feedback
		};

		var headerText = $"{typeText}: {entry.Title}";

		// Using Frame instead of Border for Android compatibility
		var card = new Frame
		{
			BackgroundColor = ResourceHelper.GetThemedColor("White", "Gray900"),
			CornerRadius = 12,
			Padding = 0,
			BorderColor = Colors.Transparent,
			HasShadow = true
		};

		var stackLayout = new VerticalStackLayout { Spacing = 0 };

		// Header - using Frame instead of Border
		var headerFrame = new Frame
		{
			BackgroundColor = ResourceHelper.GetThemedColor("Primary", "Gray700"),
			CornerRadius = 12,
			Padding = new Thickness(15, 12),
			BorderColor = Colors.Transparent,
			HasShadow = false
		};
		var headerLabel = new Label
		{
			Text = headerText,
			Style = ResourceHelper.GetStyle("LabelCardHeader")
		};
		headerFrame.Content = headerLabel;

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

		stackLayout.Children.Add(headerFrame);
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
		_selectedFilterIndex = FilterDropdown.SelectedIndex;
		LoadEntries();
	}

	private async void OnAddClicked(object? sender, EventArgs e)
	{
		await App.NavigateToAsync<SelectTicketTypePage>();
	}
	#endregion
}

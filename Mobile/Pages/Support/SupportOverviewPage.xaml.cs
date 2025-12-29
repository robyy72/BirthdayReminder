#region Usings
using Common;
#endregion

namespace Mobile;

public partial class SupportOverviewPage : ContentPage
{
	#region Fields
	private int _selectedFilterIndex = 0;
	#endregion

	#region Constructor
	public SupportOverviewPage()
	{
		InitializeComponent();
		SetupFilter();
	}
	#endregion

	#region Lifecycle
	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadEntries();
	}
	#endregion

	#region Private Methods
	private void SetupFilter()
	{
		var filterItems = new List<string>
		{
			MobileLanguages.Resources.Filter_AllMonths, // Using "-- all --" resource
			MobileLanguages.Resources.Support_Type_Bug,
			MobileLanguages.Resources.Support_Type_FeatureRequest,
			MobileLanguages.Resources.Support_Type_Feedback
		};

		FilterPicker.ItemsSource = filterItems;
		FilterPicker.SelectedIndex = 0;
	}

	private void LoadEntries()
	{
		EntriesContainer.Children.Clear();

		List<Support> entries;

		if (_selectedFilterIndex == 0)
		{
			entries = SupportService.GetAll();
		}
		else
		{
			var supportType = _selectedFilterIndex switch
			{
				1 => SupportType.Bug,
				2 => SupportType.FeatureRequest,
				3 => SupportType.Feedback,
				_ => SupportType.Feedback
			};
			entries = SupportService.GetByType(supportType);
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
				Text = MobileLanguages.Resources.EmptyView_NoBirthdays,
				Style = (Style)Application.Current!.Resources["LabelInfo"],
				HorizontalOptions = LayoutOptions.Center,
				Margin = new Thickness(0, 20, 0, 0)
			};
			EntriesContainer.Children.Add(emptyLabel);
		}
	}

	private Border CreateEntryCard(Support entry)
	{
		var typeText = ((SupportType)entry.Type) switch
		{
			SupportType.Bug => MobileLanguages.Resources.Support_Type_Bug,
			SupportType.FeatureRequest => MobileLanguages.Resources.Support_Type_FeatureRequest,
			SupportType.Feedback => MobileLanguages.Resources.Support_Type_Feedback,
			_ => MobileLanguages.Resources.Support_Type_Feedback
		};

		var headerText = $"{typeText}: {entry.Title}";

		var card = new Border
		{
			Style = (Style)Application.Current!.Resources["Card"]
		};

		var stackLayout = new VerticalStackLayout { Spacing = 0 };

		// Header
		var headerBorder = new Border
		{
			Style = (Style)Application.Current.Resources["CardHeader"]
		};
		var headerLabel = new Label
		{
			Text = headerText,
			Style = (Style)Application.Current.Resources["LabelCardHeader"]
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
			Style = (Style)Application.Current.Resources["LabelInfo"],
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
			await App.NavigateToAsync<SupportDetailPage>(entry.Id);
		};
		card.GestureRecognizers.Add(tapGesture);

		return card;
	}
	#endregion

	#region Event Handlers
	private void OnFilterChanged(object? sender, EventArgs e)
	{
		_selectedFilterIndex = FilterPicker.SelectedIndex;
		LoadEntries();
	}
	#endregion
}

using System.Windows.Input;

namespace Mobile;

public partial class SelectTimezonePage : ContentPage
{
	#region Fields
	private bool _isLoading = true;
	private List<TimezoneEntry> _timezones = [];
	#endregion

	#region Constructor
	public SelectTimezonePage()
	{
		InitializeComponent();
		TheHeader.BackClickedCommand = new Command(OnBackClicked);
		LoadTimezones();
		_isLoading = false;
	}
	#endregion

	#region Navigation
	private void OnBackClicked()
	{
		if (App.BackwardPageType != null)
		{
			var page = (Page)Activator.CreateInstance(App.BackwardPageType)!;
			App.SetRootPage(page);
		}
		else
		{
			App.SetRootPage(App.CreateMainNavigationPage());
		}
	}
	#endregion

	#region Load
	private void LoadTimezones()
	{
		_timezones = TimezoneService.LoadTimezones();
		string currentTimezoneId = App.Account.TimeZoneId ?? "";

		foreach (var tz in _timezones)
		{
			var tile = CreateTimezoneTile(tz, tz.Id == currentTimezoneId);
			TimezoneTilesContainer.Children.Add(tile);
		}
	}

	private Border CreateTimezoneTile(TimezoneEntry tz, bool isSelected)
	{
		var tile = new Border
		{
			Style = (Style)Application.Current!.Resources["Tile"]
		};

		var grid = new Grid
		{
			ColumnDefinitions =
			[
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			]
		};

		var textStack = new VerticalStackLayout
		{
			Spacing = 2,
			VerticalOptions = LayoutOptions.Center
		};

		var regionLabel = new Label
		{
			Text = tz.RegionWithOffset,
			Style = (Style)Application.Current!.Resources["LabelHeader"]
		};

		var citiesLabel = new Label
		{
			Text = tz.Cities,
			Style = (Style)Application.Current!.Resources["LabelInfoOnCard"]
		};

		textStack.Children.Add(regionLabel);
		textStack.Children.Add(citiesLabel);
		grid.SetColumn(textStack, 0);

		var radio = new RadioButton
		{
			IsChecked = isSelected,
			Value = tz.Id,
			Style = (Style)Application.Current!.Resources["RadioStandard"],
			VerticalOptions = LayoutOptions.Center
		};
		radio.CheckedChanged += OnTimezoneRadioChanged;
		grid.SetColumn(radio, 1);

		grid.Children.Add(textStack);
		grid.Children.Add(radio);
		tile.Content = grid;

		return tile;
	}
	#endregion

	#region Events
	private void OnTimezoneRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value)
			return;

		if (sender is RadioButton radio && radio.Value is string timezoneId)
		{
			App.Account.TimeZoneId = timezoneId;
			AccountService.Save();
		}
	}
	#endregion
}

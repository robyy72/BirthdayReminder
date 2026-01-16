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
			RowDefinitions =
			[
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto)
			],
			ColumnDefinitions =
			[
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			],
			RowSpacing = 4
		};

		var regionLabel = new Label
		{
			Text = tz.RegionWithOffset,
			Style = (Style)Application.Current!.Resources["LabelHeader"],
			VerticalOptions = LayoutOptions.Center
		};
		grid.SetRow(regionLabel, 0);
		grid.SetColumn(regionLabel, 0);

		var radio = new RadioButton
		{
			IsChecked = isSelected,
			Value = tz.Id,
			Style = (Style)Application.Current!.Resources["RadioStandard"],
			VerticalOptions = LayoutOptions.Center
		};
		radio.CheckedChanged += OnTimezoneRadioChanged;
		grid.SetRow(radio, 0);
		grid.SetColumn(radio, 1);

		var citiesLabel = new Label
		{
			Text = tz.Cities,
			Style = (Style)Application.Current!.Resources["LabelInfoOnCard"]
		};
		grid.SetRow(citiesLabel, 1);
		grid.SetColumn(citiesLabel, 0);
		grid.SetColumnSpan(citiesLabel, 2);

		grid.Children.Add(regionLabel);
		grid.Children.Add(radio);
		grid.Children.Add(citiesLabel);
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

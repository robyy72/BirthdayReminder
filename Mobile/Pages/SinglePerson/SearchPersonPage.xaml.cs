namespace Mobile;

public partial class SearchPersonPage : ContentPage
{
	#region Fields
	private List<Person> _allPersons = [];
	#endregion

	#region Constructor
	public SearchPersonPage()
	{
		InitializeComponent();
		LoadPersons();
	}
	#endregion

	#region Private Methods
	private void LoadPersons()
	{
		_allPersons = App.Persons
			.OrderBy(p => p.DisplayName)
			.ToList();
	}

	private void SearchAndDisplayResults()
	{
		ResultsList.Children.Clear();
		ResultsCard.IsVisible = false;

		string firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
		string lastName = LastNameEntry.Text?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
		{
			return;
		}

		var matches = _allPersons
			.Where(p =>
			{
				bool firstNameMatch = string.IsNullOrEmpty(firstName) ||
					(!string.IsNullOrEmpty(p.FirstName) && p.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase));
				bool lastNameMatch = string.IsNullOrEmpty(lastName) ||
					(!string.IsNullOrEmpty(p.LastName) && p.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));

				return firstNameMatch && lastNameMatch;
			})
			.Take(10)
			.ToList();

		if (matches.Count == 0)
		{
			return;
		}

		foreach (var person in matches)
		{
			var tile = CreatePersonTile(person);
			ResultsList.Children.Add(tile);
		}

		ResultsCard.IsVisible = true;
	}

	private Border CreatePersonTile(Person person)
	{
		var nameDirection = App.Account.PersonNameDirection;
		string displayName = PersonHelper.GetDisplayName(person.FirstName, person.LastName, nameDirection);
		string dateDisplay = person.Birthday != null ? BirthdayHelper.GetDateDisplay(person.Birthday) : string.Empty;

		var tile = new Border
		{
			Style = (Style)Application.Current!.Resources["Tile"]
		};

		var grid = new Grid
		{
			ColumnDefinitions = new ColumnDefinitionCollection
			{
				new ColumnDefinition { Width = GridLength.Star },
				new ColumnDefinition { Width = GridLength.Auto }
			},
			Padding = new Thickness(15, 12)
		};

		var nameLabel = new Label
		{
			Text = displayName,
			Style = (Style)Application.Current.Resources["LabelInfo"],
			VerticalOptions = LayoutOptions.Center
		};

		var dateLabel = new Label
		{
			Text = dateDisplay,
			Style = (Style)Application.Current.Resources["LabelDescription"],
			VerticalOptions = LayoutOptions.Center
		};
		Grid.SetColumn(dateLabel, 1);

		grid.Children.Add(nameLabel);
		grid.Children.Add(dateLabel);

		tile.Content = grid;

		var tapGesture = new TapGestureRecognizer();
		int personId = person.Id;
		tapGesture.Tapped += async (s, e) =>
		{
			await App.NavigateToAsync<EditPersonPage>(personId);
		};
		tile.GestureRecognizers.Add(tapGesture);

		return tile;
	}
	#endregion

	#region Event Handlers
	private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
	{
		SearchAndDisplayResults();
	}

	private async void OnCreateNewClicked(object? sender, EventArgs e)
	{
		string firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
		string lastName = LastNameEntry.Text?.Trim() ?? string.Empty;

		string prefillName = string.Empty;
		if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
		{
			prefillName = $"{firstName} {lastName}";
		}
		else if (!string.IsNullOrEmpty(firstName))
		{
			prefillName = firstName;
		}
		else if (!string.IsNullOrEmpty(lastName))
		{
			prefillName = lastName;
		}

		await App.NavigateToAsync<CreatePersonPage>(prefillName);
	}

	private void OnEntryFocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			var primary = (Color)Application.Current!.Resources["Primary"];
			outerBorder.Stroke = primary;
			outerBorder.StrokeThickness = 2;
		}
	}

	private void OnEntryUnfocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			var strokeColor = Application.Current!.RequestedTheme == AppTheme.Dark
				? (Color)Application.Current.Resources["Gray700"]
				: (Color)Application.Current.Resources["Gray300"];
			outerBorder.Stroke = strokeColor;
			outerBorder.StrokeThickness = 1;
		}
	}
	#endregion
}

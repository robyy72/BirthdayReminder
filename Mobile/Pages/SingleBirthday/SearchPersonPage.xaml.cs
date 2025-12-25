namespace Mobile;

public partial class SearchPersonPage : ContentPage
{
	private List<Person> _allPersons = [];
	private List<Person> _filteredPersons = [];

	public SearchPersonPage()
	{
		InitializeComponent();
		LoadPersons();
	}

	private void LoadPersons()
	{
		// Get all persons ordered by name
		_allPersons = App.Persons
			.OrderBy(p => p.DisplayName)
			.ToList();
	}

	private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
	{
		string searchText = e.NewTextValue?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(searchText))
		{
			ResultsList.IsVisible = false;
			ResultsList.ItemsSource = null;
			return;
		}

		// Filter persons by name
		_filteredPersons = _allPersons
			.Where(p => p.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
			.Take(20)
			.ToList();

		ResultsList.ItemsSource = _filteredPersons;
		ResultsList.IsVisible = true;
	}

	private async void OnPersonSelected(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.Count == 0)
			return;

		var selectedPerson = e.CurrentSelection[0] as Person;
		if (selectedPerson == null)
			return;

		// Clear selection
		ResultsList.SelectedItem = null;

		// Navigate to EditBirthday with this person's ID
		await NavigationService.NavigateTo<EditBirthdayPage>(selectedPerson.Id);
	}

	private async void OnCreateNewClicked(object? sender, EventArgs e)
	{
		// Navigate to CreateBirthday with search text as prefill
		string searchText = SearchEntry.Text?.Trim() ?? string.Empty;
		await NavigationService.NavigateTo<CreateBirthdayPage>(searchText);
	}
}

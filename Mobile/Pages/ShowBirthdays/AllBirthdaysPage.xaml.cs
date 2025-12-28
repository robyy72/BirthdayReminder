namespace Mobile;

#region Usings
using MobileLanguages;
#endregion

public partial class AllBirthdaysPage : ContentPage
{
	#region Private Class Variables
	private List<PersonViewModel> _allBirthdays = [];
	private int _selectedMonthIndex = 0;
	#endregion

	#region Properties
	public List<PersonViewModel> FilteredBirthdays { get; set; } = [];
	#endregion

	public AllBirthdaysPage()
	{
		InitializeComponent();
		BindingContext = this;
		InitializeMonthPicker();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		App.BackwardPageType = typeof(AllBirthdaysPage);
		LoadBirthdays();
		ApplyFilter();
	}

	private void InitializeMonthPicker()
	{
		var months = new List<string> { MobileLanguages.Resources.Filter_AllMonths };

		for (int i = 1; i <= 12; i++)
		{
			months.Add(LanguageHelper.GetFullMonthName(i));
		}

		MonthPicker.ItemsSource = months;

		int currentMonth = DateTime.Today.Month;
		_selectedMonthIndex = currentMonth;
		MonthPicker.SelectedIndex = currentMonth;
	}

	private void LoadBirthdays()
	{
		var nameDirection = App.Account.PersonNameDirection;

		_allBirthdays = App.Persons
			.Where(p => p.Birthday != null)
			.OrderBy(p => p.Birthday!.Month)
			.ThenBy(p => p.Birthday!.Day)
			.Select(p => new PersonViewModel(p, nameDirection))
			.ToList();
	}

	private void ApplyFilter()
	{
		if (_selectedMonthIndex == 0)
		{
			FilteredBirthdays = _allBirthdays;
		}
		else
		{
			FilteredBirthdays = _allBirthdays
				.Where(p => p.BirthdayMonth == _selectedMonthIndex)
				.ToList();
		}

		OnPropertyChanged(nameof(FilteredBirthdays));
	}

	private void OnMonthFilterChanged(object? sender, EventArgs e)
	{
		_selectedMonthIndex = MonthPicker.SelectedIndex;
		ApplyFilter();
	}

	private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
		{
			await App.NavigateToAsync<DetailBirthdayPage>(vm.Id);
		}

		if (sender is CollectionView cv)
		{
			cv.SelectedItem = null;
		}
	}

	private async void OnNewBirthdayClicked(object? sender, EventArgs e)
	{
		// If contacts are not used (None), go directly to CreateBirthday
		// Otherwise show search page to avoid duplicates
		if (App.Account.ContactsReadMode == ContactsReadMode.None)
		{
			await App.NavigateToAsync<CreateBirthdayPage>();
		}
		else
		{
			await App.NavigateToAsync<SearchPersonPage>();
		}
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await App.GoBackAsync();
	}
}

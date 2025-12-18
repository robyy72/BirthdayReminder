namespace Mobile;

#region Usings
using MobileLanguages;
#endregion

public partial class AllBirthdaysPage : ContentPage
{
	#region Private Class Variables
	private readonly Settings _settings = SettingsService.Get();
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
		var allPersons = new List<Person>();
		var nameDirection = _settings.PersonNameDirection;

		for (int month = 1; month <= 12; month++)
		{
			allPersons.AddRange(BirthdayService.GetByMonth(month));
		}

		_allBirthdays = allPersons
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
			await Shell.Current.GoToAsync($"{nameof(CreateEditBirthdayPage)}?Id={vm.Id}");
		}

		if (sender is CollectionView cv)
		{
			cv.SelectedItem = null;
		}
	}

	private async void OnNewBirthdayClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(CreateEditBirthdayPage));
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}

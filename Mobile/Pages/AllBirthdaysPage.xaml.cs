namespace Mobile;

public partial class AllBirthdaysPage : ContentPage
{
	public List<PersonViewModel> AllBirthdays { get; set; } = [];

	public AllBirthdaysPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadBirthdays();
	}

	private void LoadBirthdays()
	{
		var allPersons = new List<Person>();

		for (int month = 1; month <= 12; month++)
		{
			allPersons.AddRange(BirthdayService.GetByMonth(month));
		}

		AllBirthdays = allPersons
			.Where(p => p.Birthday != null)
			.OrderBy(p => p.Birthday!.Month)
			.ThenBy(p => p.Birthday!.Day)
			.Select(p => new PersonViewModel(p))
			.ToList();

		OnPropertyChanged(nameof(AllBirthdays));
	}

	private async void OnBirthdaySelected(object? sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is PersonViewModel vm)
		{
			await Shell.Current.GoToAsync($"{nameof(NewBirthdayPage)}?Id={vm.Id}");
		}

		if (sender is CollectionView cv)
		{
			cv.SelectedItem = null;
		}
	}
}

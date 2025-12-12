namespace Mobile;

public partial class AllBirthdaysPage : ContentPage
{
	#region Private Class Variables
	private readonly Settings _settings = SettingsService.Get();
	#endregion

	#region Properties
	public List<PersonViewModel> AllBirthdays { get; set; } = [];
	#endregion

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
		var nameDirection = _settings.PersonNameDirection;

		for (int month = 1; month <= 12; month++)
		{
			allPersons.AddRange(BirthdayService.GetByMonth(month));
		}

		AllBirthdays = allPersons
			.Where(p => p.Birthday != null)
			.OrderBy(p => p.Birthday!.Month)
			.ThenBy(p => p.Birthday!.Day)
			.Select(p => new PersonViewModel(p, nameDirection))
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

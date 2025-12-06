namespace Mobile;

public partial class MainPage : ContentPage
{
	public List<PersonViewModel> UpcomingBirthdays { get; set; } = [];
	public List<PersonViewModel> MissedBirthdays { get; set; } = [];

	public MainPage()
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
		var today = DateTime.Today;
		var allPersons = new List<Person>();

		for (int month = 1; month <= 12; month++)
		{
			allPersons.AddRange(BirthdayService.GetByMonth(month));
		}

		var upcoming = allPersons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysUntil = GetDaysUntilBirthday(p.Birthday!, today) })
			.Where(x => x.DaysUntil >= 0)
			.OrderBy(x => x.DaysUntil)
			.Take(MobileConstants.SHOW_UPCOMING)
			.Select(x => new PersonViewModel(x.Person))
			.ToList();

		var missed = allPersons
			.Where(p => p.Birthday != null)
			.Select(p => new { Person = p, DaysSince = GetDaysSinceBirthday(p.Birthday!, today) })
			.Where(x => x.DaysSince > 0 && x.DaysSince <= 30)
			.OrderBy(x => x.DaysSince)
			.Take(MobileConstants.SHOW_MISSED_BIRTHDAYS)
			.Select(x => new PersonViewModel(x.Person))
			.ToList();

		UpcomingBirthdays = upcoming;
		MissedBirthdays = missed;

		OnPropertyChanged(nameof(UpcomingBirthdays));
		OnPropertyChanged(nameof(MissedBirthdays));
	}

	private static int GetDaysUntilBirthday(Birthday birthday, DateTime today)
	{
		var thisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
		if (thisYear < today)
		{
			thisYear = thisYear.AddYears(1);
		}
		return (thisYear - today).Days;
	}

	private static int GetDaysSinceBirthday(Birthday birthday, DateTime today)
	{
		var thisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
		if (thisYear > today)
		{
			thisYear = thisYear.AddYears(-1);
		}
		var days = (today - thisYear).Days;
		return days > 0 && days <= 30 ? days : 0;
	}

	private async void OnNewBirthdayClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(NewBirthdayPage));
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

public class PersonViewModel
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string BirthdayDisplay { get; set; } = string.Empty;

	public PersonViewModel(Person person)
	{
		Id = person.Id;
		Name = person.Name;
		if (person.Birthday != null)
		{
			BirthdayDisplay = $"{person.Birthday.Day:00}.{person.Birthday.Month:00}.{person.Birthday.Year}";
		}
	}
}

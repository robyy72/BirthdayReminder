#region Usings
using Common;
#endregion

namespace Mobile;

public partial class CreatePersonPage : ContentPage
{
    #region Fields
    private List<string> _days = new();
    private List<string> _months = new();
    private List<string> _years = new();
    private string? _prefillName;
    #endregion

    #region Constructor
    public CreatePersonPage() : this(null)
    {
    }

    public CreatePersonPage(string? prefillName)
    {
        _prefillName = prefillName;
        InitializeComponent();
        InitializePickers();
        SetDefaultValues();
        PrefillName();
    }
    #endregion

    #region Initialization
    private void InitializePickers()
    {
        for (int i = 1; i <= 31; i++)
        {
            _days.Add(i.ToString());
        }
        DayPicker.ItemsSource = _days;

        _months = new List<string>
        {
            MobileLanguages.Resources.Month_Short_Jan,
            MobileLanguages.Resources.Month_Short_Feb,
            MobileLanguages.Resources.Month_Short_Mar,
            MobileLanguages.Resources.Month_Short_Apr,
            MobileLanguages.Resources.Month_Short_May,
            MobileLanguages.Resources.Month_Short_Jun,
            MobileLanguages.Resources.Month_Short_Jul,
            MobileLanguages.Resources.Month_Short_Aug,
            MobileLanguages.Resources.Month_Short_Sep,
            MobileLanguages.Resources.Month_Short_Oct,
            MobileLanguages.Resources.Month_Short_Nov,
            MobileLanguages.Resources.Month_Short_Dec
        };
        MonthPicker.ItemsSource = _months;

        int currentYear = DateTime.Now.Year;
        int minYear = currentYear - MobileConstants.MAX_AGE;
        _years.Add("0000");
        for (int year = currentYear; year >= minYear; year--)
        {
            _years.Add(year.ToString());
        }
        YearPicker.ItemsSource = _years;
    }

    private void SetDefaultValues()
    {
        DayPicker.SelectedIndex = DateTime.Now.Day - 1;
        MonthPicker.SelectedIndex = DateTime.Now.Month - 1;
        YearPicker.SelectedIndex = 0;
    }

    private void PrefillName()
    {
        if (string.IsNullOrWhiteSpace(_prefillName))
            return;

        // Try to split into first and last name
        var parts = _prefillName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            FirstNameEntry.Text = parts[0];
            LastNameEntry.Text = parts[1];
        }
        else
        {
            FirstNameEntry.Text = _prefillName.Trim();
        }
    }
    #endregion

    #region Event Handlers
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
        var lastName = LastNameEntry.Text?.Trim() ?? string.Empty;

        // Validate: at least one name required
        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            ErrorLabel.Text = MobileLanguages.Resources.Error_NameRequired;
            ErrorBorder.IsVisible = true;
            return;
        }

        ErrorBorder.IsVisible = false;

        // First entry: set PersonNameDirection if still NotSet
        if (App.Persons.Count == 0 && App.Account.PersonNameDirection == PersonNameDirection.NotSet)
        {
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                App.Account.PersonNameDirection = PersonNameDirection.FirstFirstName;
            else if (!string.IsNullOrEmpty(firstName))
                App.Account.PersonNameDirection = PersonNameDirection.FirstNameOnly;
            else
                App.Account.PersonNameDirection = PersonNameDirection.FirstLastName;

            AccountService.Save();
        }

        int selectedDay = DayPicker.SelectedIndex + 1;
        int selectedMonth = MonthPicker.SelectedIndex + 1;
        string selectedYearStr = _years[YearPicker.SelectedIndex];
        int selectedYear = selectedYearStr == "0000" ? 0 : int.Parse(selectedYearStr);

        var birthday = new Birthday
        {
            Day = selectedDay,
            Month = selectedMonth,
            Year = selectedYear
        };

        var person = new Person
        {
            Id = GenerateId(),
            FirstName = firstName,
            LastName = lastName,
            Birthday = birthday
        };

        PersonService.Add(person);

        // Navigate to DetailPersonPage instead of going back
        await App.NavigateToAsync<DetailPersonPage>(person.Id);
    }

    private async void OnHelpBirthdayTapped(object? sender, EventArgs e)
    {
        var helpPage = new HelpPage { Topic = HelpTopic.BirthdayWithoutYear.ToString() };
        await Navigation.PushModalAsync(helpPage);
    }

    private async void OnWriteToContactToggled(object? sender, ToggledEventArgs e)
    {
        if (!e.Value)
        {
            return;
        }

        bool granted = await DeviceService.RequestContactsWritePermissionAsync();
        if (!granted)
        {
            WriteToContactSwitch.IsToggled = false;
            await DisplayAlert(
                MobileLanguages.Resources.Permission_Required,
                MobileLanguages.Resources.Permission_ContactsWrite_Denied,
                MobileLanguages.Resources.General_Button_OK);
        }
    }

    private void OnDatePickerChanged(object? sender, EventArgs e)
    {
        SearchMatchingBirthdays();
    }

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
        {
            outerBorder.Stroke = ResourceHelper.GetColor("Primary");
            outerBorder.StrokeThickness = 2;
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
        {
            outerBorder.Stroke = ResourceHelper.GetThemedColor("Gray300", "Gray700");
            outerBorder.StrokeThickness = 1;
        }
    }
    #endregion

    #region Private Methods
    private static int GenerateId()
    {
        int id = (int)(DateTime.Now.Ticks % int.MaxValue);
        return id;
    }

    private void SearchMatchingBirthdays()
    {
        FoundBirthdaysList.Children.Clear();
        FoundBirthdaysCard.IsVisible = false;

        if (DayPicker.SelectedIndex < 0 || MonthPicker.SelectedIndex < 0)
            return;

        int day = DayPicker.SelectedIndex + 1;
        int month = MonthPicker.SelectedIndex + 1;

        var matches = App.Persons
            .Where(p => p.Birthday != null && p.Birthday.Day == day && p.Birthday.Month == month)
            .OrderBy(p => p.DisplayName)
            .ToList();

        if (matches.Count == 0)
            return;

        var nameDirection = App.Account.PersonNameDirection;

        foreach (var person in matches)
        {
            string displayName = PersonHelper.GetDisplayName(person.FirstName, person.LastName, nameDirection);
            string dateDisplay = person.Birthday != null ? BirthdayHelper.GetDateDisplay(person.Birthday) : string.Empty;

            var tile = new TilePersonResult
            {
                DisplayName = displayName,
                DateDisplay = dateDisplay,
                PersonId = person.Id
            };

            FoundBirthdaysList.Children.Add(tile);
        }

        FoundBirthdaysCard.IsVisible = true;
    }
    #endregion
}

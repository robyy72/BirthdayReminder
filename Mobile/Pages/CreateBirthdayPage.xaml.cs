#region Usings
using Common;
#endregion

namespace Mobile;

public partial class CreateBirthdayPage : ContentPage
{
    #region Fields
    private List<string> _days = new();
    private List<string> _months = new();
    private List<string> _years = new();
    private string? _prefillName;
    #endregion

    #region Constructor
    public CreateBirthdayPage() : this(null)
    {
    }

    public CreateBirthdayPage(string? prefillName)
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

        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            return;
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

        await NavigationService.GoBack();
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

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            Border? border = entry.Parent as Border;
            if (border != null)
            {
                border.Stroke = Color.FromArgb("#0066CC");
                border.StrokeThickness = 2;
            }
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            Border? border = entry.Parent as Border;
            if (border != null)
            {
                border.Stroke = Color.FromArgb("#ACACAC");
                border.StrokeThickness = 1;
            }
        }
    }

    private void OnDatePickerChanged(object? sender, EventArgs e)
    {
        SearchMatchingBirthdays();
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

        foreach (var person in matches)
        {
            var label = new Label
            {
                Text = person.DisplayName,
                Style = (Style)Application.Current!.Resources["LabelInfo"],
                Padding = new Thickness(10, 8),
                TextDecorations = TextDecorations.Underline
            };

            var tapGesture = new TapGestureRecognizer();
            int personId = person.Id;
            tapGesture.Tapped += async (s, e) =>
            {
                await NavigationService.NavigateTo<EditBirthdayPage>(personId);
            };
            label.GestureRecognizers.Add(tapGesture);

            FoundBirthdaysList.Children.Add(label);
        }

        FoundBirthdaysCard.IsVisible = true;
    }
    #endregion
}

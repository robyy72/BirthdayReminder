#region Usings
using Common;
#endregion

namespace Mobile;

public partial class EditBirthdayPage : ContentPage
{
    #region Fields
    private int _personId;
    private Person? _person;
    private List<string> _days = new();
    private List<string> _months = new();
    private List<string> _years = new();
    #endregion

    #region Constructor
    public EditBirthdayPage(int personId)
    {
        InitializeComponent();
        _personId = personId;
        InitializePickers();
    }
    #endregion

    #region Lifecycle
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPerson();
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

    private async Task LoadPerson()
    {
        _person = PersonService.GetById(_personId);
        if (_person == null)
        {
            await App.GoBackAsync();
            return;
        }

        FirstNameEntry.Text = _person.FirstName;
        LastNameEntry.Text = _person.LastName;

        if (_person.Birthday != null)
        {
            DayPicker.SelectedIndex = _person.Birthday.Day - 1;
            MonthPicker.SelectedIndex = _person.Birthday.Month - 1;

            int year = _person.Birthday.Year;
            if (BirthdayHelper.ShouldDisplayYear(year))
            {
                int yearIndex = _years.IndexOf(year.ToString());
                YearPicker.SelectedIndex = yearIndex >= 0 ? yearIndex : 0;
            }
            else
            {
                YearPicker.SelectedIndex = 0;
            }
        }

        // Contact toggle
        UpdateContactRow.IsVisible = true;
        if (!string.IsNullOrEmpty(_person.ContactId))
        {
            UpdateContactLabel.Text = MobileLanguages.Resources.Settings_UpdateContact;
        }
        else
        {
            UpdateContactLabel.Text = MobileLanguages.Resources.Settings_CreateContact;
        }
    }
    #endregion

    #region Event Handlers
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_person == null)
        {
            return;
        }

        var firstName = FirstNameEntry.Text?.Trim() ?? string.Empty;
        var lastName = LastNameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
        {
            ErrorLabel.Text = MobileLanguages.Resources.Error_NameRequired;
            ErrorBorder.IsVisible = true;
            return;
        }

        ErrorBorder.IsVisible = false;

        int selectedDay = DayPicker.SelectedIndex + 1;
        int selectedMonth = MonthPicker.SelectedIndex + 1;
        string selectedYearStr = _years[YearPicker.SelectedIndex];
        int selectedYear = selectedYearStr == "0000" ? 0 : int.Parse(selectedYearStr);

        _person.FirstName = firstName;
        _person.LastName = lastName;
        _person.Birthday = new Birthday
        {
            Day = selectedDay,
            Month = selectedMonth,
            Year = selectedYear
        };

        PersonService.Update(_person);

        await App.GoBackAsync();
    }

    private async void OnHelpBirthdayTapped(object? sender, EventArgs e)
    {
        var helpPage = new HelpPage { Topic = HelpTopic.BirthdayWithoutYear.ToString() };
        await Navigation.PushModalAsync(helpPage);
    }

    private async void OnUpdateContactToggled(object? sender, ToggledEventArgs e)
    {
        if (!e.Value)
        {
            return;
        }

        bool granted = await DeviceService.RequestContactsWritePermissionAsync();
        if (!granted)
        {
            UpdateContactSwitch.IsToggled = false;
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
    #endregion
}

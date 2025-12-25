#region Usings
using Common;
#endregion

namespace Mobile;

public partial class DetailBirthdayPage : ContentPage
{
    #region Fields
    private int _personId;
    private Person? _person;
    #endregion

    #region Constructor
    public DetailBirthdayPage(int personId)
    {
        InitializeComponent();
        _personId = personId;
    }
    #endregion

    #region Lifecycle
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPerson();
    }
    #endregion

    #region Private Methods
    private async Task LoadPerson()
    {
        _person = PersonService.GetById(_personId);
        if (_person == null)
        {
            await NavigationService.GoBack();
            return;
        }

        // Name
        string displayName = PersonHelper.GetDisplayName(
            _person.FirstName,
            _person.LastName,
            App.Account.PersonNameDirection);
        NameDisplayLabel.Text = displayName;

        // Birthday
        if (_person.Birthday != null)
        {
            string birthdayDisplay = BirthdayHelper.GetDateDisplay(_person.Birthday);
            BirthdayDisplayLabel.Text = birthdayDisplay;

            // Age
            if (BirthdayHelper.ShouldDisplayYear(_person.Birthday.Year))
            {
                int age = BirthdayHelper.CalculateAge(_person.Birthday);
                AgeDisplayLabel.Text = string.Format(MobileLanguages.Resources.Age_Years, age);
                AgeDisplayLabel.IsVisible = true;
            }
            else
            {
                AgeDisplayLabel.IsVisible = false;
            }

            // Days until
            int daysUntil = BirthdayHelper.GetDaysUntilBirthday(_person.Birthday, DateTime.Today);
            if (daysUntil == 0)
            {
                DaysUntilLabel.Text = MobileLanguages.Resources.Birthday_Today;
            }
            else if (daysUntil == 1)
            {
                DaysUntilLabel.Text = MobileLanguages.Resources.Birthday_Tomorrow;
            }
            else
            {
                DaysUntilLabel.Text = string.Format(MobileLanguages.Resources.Birthday_InDays, daysUntil);
            }
        }
    }
    #endregion

    #region Event Handlers
    private async void OnEditClicked(object? sender, EventArgs e)
    {
        await NavigationService.NavigateTo<EditBirthdayPage>(_personId);
    }

    private async void OnEditMenuClicked(object? sender, EventArgs e)
    {
        await NavigationService.NavigateTo<EditBirthdayPage>(_personId);
    }

    private async void OnDeleteMenuClicked(object? sender, EventArgs e)
    {
        await NavigationService.NavigateTo<DeleteBirthdayPage>(_personId);
    }
    #endregion
}

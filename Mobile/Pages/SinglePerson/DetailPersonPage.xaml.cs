#region Usings
using Common;
using System.Windows.Input;
#endregion

namespace Mobile;

public partial class DetailPersonPage : ContentPage
{
    #region Fields
    private int _personId;
    private Person? _person;
    #endregion

    #region Properties
    public ICommand BackCommand { get; }
    #endregion

    #region Constructor
    public DetailPersonPage(int personId)
    {
        BackCommand = new Command(async () => await NavigateBack());
        InitializeComponent();
        BindingContext = this;
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
            await App.GoBackAsync();
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
            string dateDisplay = BirthdayHelper.GetDateDisplay(_person.Birthday);

            // Date
            DateDisplayLabel.Text = dateDisplay;

            // Age (only if year is known)
            if (BirthdayHelper.ShouldDisplayYear(_person.Birthday.Year))
            {
                int nextAge = BirthdayHelper.CalculateAge(_person.Birthday) + 1;
                AgeDisplayLabel.Text = string.Format(
                    MobileLanguages.Resources.Detail_AgeOnThatDay,
                    nextAge);
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
                DaysUntilLabel.Text = MobileLanguages.Resources.Detail_Birthday_TodayIs;
            }
            else if (daysUntil == 1)
            {
                DaysUntilLabel.Text = MobileLanguages.Resources.Detail_Birthday_TomorrowIs;
            }
            else
            {
                DaysUntilLabel.Text = string.Format(
                    MobileLanguages.Resources.Detail_Birthday_InDaysIs,
                    daysUntil);
            }
        }

        // Reminder info
        bool hasReminders = _person.Reminders.Any(r => r != null);
        ReminderInfoLabel.Text = hasReminders
            ? MobileLanguages.Resources.Detail_RemindersSet
            : MobileLanguages.Resources.Detail_NoReminders;
    }
    #endregion

    #region Navigation
    private async Task NavigateBack()
    {
        // Navigate to root first
        await App.NavigateToRootAsync();

        // If coming from AllBirthdaysPage, navigate there
        if (App.BackwardPageType == typeof(AllBirthdaysPage))
        {
            await App.NavigateToAsync<AllBirthdaysPage>();
        }
    }
    #endregion

    #region Event Handlers
    private async void OnEditClicked(object? sender, EventArgs e)
    {
        await App.NavigateToAsync<EditPersonPage>(_personId);
    }

    private async void OnEditMenuClicked(object? sender, EventArgs e)
    {
        await App.NavigateToAsync<EditPersonPage>(_personId);
    }

    private async void OnDeleteMenuClicked(object? sender, EventArgs e)
    {
        await App.NavigateToAsync<DeletePersonPage>(_personId);
    }
    #endregion
}

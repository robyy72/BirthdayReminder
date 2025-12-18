#region Usings
#endregion

namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class CreateEditBirthdayPage_2 : ContentPage
{
	private int? _personId;
	private Person? _person;

	public string PersonId
	{
		set
		{
			if (int.TryParse(value, out var id))
			{
				_personId = id;
			}
		}
	}

	public CreateEditBirthdayPage_2()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (_personId.HasValue)
		{
			LoadPerson(_personId.Value);
		}
	}

	private void LoadPerson(int id)
	{
		_person = BirthdayService.GetPerson(id);
		if (_person == null)
		{
			Shell.Current.GoToAsync("..");
			return;
		}

		ReminderEmailSwitch.IsToggled = _person.ReminderEmailEnabled;
		ReminderSmsSwitch.IsToggled = _person.ReminderSmsEnabled;
		ReminderLockScreenSwitch.IsToggled = _person.ReminderLockScreenEnabled;
		ReminderWhatsAppSwitch.IsToggled = _person.ReminderWhatsAppEnabled;
		RemindUntilApprovedSwitch.IsToggled = _person.RemindUntilApproved;
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		if (_person == null)
			return;

		_person.ReminderEmailEnabled = ReminderEmailSwitch.IsToggled;
		_person.ReminderSmsEnabled = ReminderSmsSwitch.IsToggled;
		_person.ReminderLockScreenEnabled = ReminderLockScreenSwitch.IsToggled;
		_person.ReminderWhatsAppEnabled = ReminderWhatsAppSwitch.IsToggled;
		_person.RemindUntilApproved = RemindUntilApprovedSwitch.IsToggled;

		BirthdayService.Update(_person);
		NotificationService.ScheduleForPerson(_person);

		App.NeedsReloadBirthdays = true;
		await Shell.Current.GoToAsync("../..");
	}

	private async void OnBackClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}

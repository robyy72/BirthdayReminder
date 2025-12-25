#region Usings
using Common;
#endregion

namespace Mobile;

public partial class DeleteBirthdayPage : ContentPage
{
	private int? _personId;
	private Person? _person;

	public DeleteBirthdayPage()
	{
		InitializeComponent();
	}

	public DeleteBirthdayPage(int personId) : this()
	{
		_personId = personId;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (_personId.HasValue)
		{
			LoadPerson(_personId.Value);
		}
		else
		{
			await NavigationService.GoBack();
		}
	}

	private async void LoadPerson(int id)
	{
		_person = BirthdayService.GetPerson(id);
		if (_person == null)
		{
			await NavigationService.GoBack();
			return;
		}

		string displayName = PersonHelper.GetDisplayName(
			_person.FirstName,
			_person.LastName,
			PersonNameDirection.FirstFirstName);
		NameDisplayLabel.Text = displayName;

		if (_person.Birthday != null)
		{
			string birthdayDisplay = BirthdayHelper.GetDateDisplay(_person.Birthday);
			BirthdayDisplayLabel.Text = birthdayDisplay;
		}

		if (!string.IsNullOrEmpty(_person.ContactId))
		{
			DeleteFromContactRow.IsVisible = true;
			DeleteFromContactLabel.Text = MobileLanguages.Resources.Settings_DeleteContact;
		}
	}

	private async void OnCancelClicked(object? sender, EventArgs e)
	{
		await NavigationService.GoBack();
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_person == null)
		{
			return;
		}

		NotificationService.CancelForPerson(_person.Id);
		BirthdayService.Remove(_person.Id);
		App.NeedsReloadBirthdays = true;
		await NavigationService.NavigateToRoot();
	}
}

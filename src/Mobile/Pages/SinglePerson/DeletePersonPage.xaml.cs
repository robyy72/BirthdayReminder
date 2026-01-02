#region Usings
using Common;
#endregion

namespace Mobile;

public partial class DeletePersonPage : ContentPage
{
	private int? _personId;
	private Person? _person;

	public DeletePersonPage()
	{
		InitializeComponent();
	}

	public DeletePersonPage(int personId) : this()
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
			await App.GoBackAsync();
		}
	}

	private async void LoadPerson(int id)
	{
		_person = PersonService.GetById(id);
		if (_person == null)
		{
			await App.GoBackAsync();
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
		await App.GoBackAsync();
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_person == null)
		{
			return;
		}

		NotificationService.CancelForPerson(_person.Id);
		PersonService.Remove(_person.Id);
		await App.NavigateToRootAsync();
	}
}

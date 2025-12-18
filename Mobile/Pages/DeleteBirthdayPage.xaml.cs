#region Usings
using Common;
#endregion

namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class DeleteBirthdayPage : ContentPage
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

	public DeleteBirthdayPage()
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
		else
		{
			Shell.Current.GoToAsync("..");
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
		await Shell.Current.GoToAsync("..");
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
		await Shell.Current.GoToAsync("../..");
	}
}

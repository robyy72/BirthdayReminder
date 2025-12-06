namespace Mobile;

[QueryProperty(nameof(PersonId), "Id")]
public partial class NewBirthdayPage : ContentPage
{
	private int? _personId;
	private Person? _existingPerson;

	public string PersonId
	{
		set
		{
			if (int.TryParse(value, out var id))
			{
				_personId = id;
				LoadPerson(id);
			}
		}
	}

	public NewBirthdayPage()
	{
		InitializeComponent();
	}

	private void LoadPerson(int id)
	{
		_existingPerson = BirthdayService.GetPerson(id);
		if (_existingPerson != null)
		{
			NameEntry.Text = _existingPerson.Name;
			if (_existingPerson.Birthday != null)
			{
				BirthdayPicker.Date = new DateTime(
					_existingPerson.Birthday.Year,
					_existingPerson.Birthday.Month,
					_existingPerson.Birthday.Day);
			}
			DeleteButton.IsVisible = true;
		}
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var name = NameEntry.Text?.Trim();
		if (string.IsNullOrEmpty(name))
		{
			return;
		}

		var birthday = new Birthday(
			BirthdayPicker.Date.Day,
			BirthdayPicker.Date.Month,
			BirthdayPicker.Date.Year);

		if (_existingPerson != null)
		{
			_existingPerson.Name = name;
			_existingPerson.Birthday = birthday;
			BirthdayService.Update(_existingPerson);
		}
		else
		{
			var person = new Person
			{
				Id = GenerateId(),
				Name = name,
				Birthday = birthday
			};
			BirthdayService.Add(person);
		}

		await Shell.Current.GoToAsync("..");
	}

	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_personId.HasValue)
		{
			BirthdayService.Remove(_personId.Value);
			await Shell.Current.GoToAsync("..");
		}
	}

	private static int GenerateId()
	{
		return (int)(DateTime.Now.Ticks % int.MaxValue);
	}
}

namespace Mobile;

/// <summary>
/// Aim: Service for managing birthdays stored by month with person references.
/// </summary>
public static class BirthdayService
{
	private static Dictionary<int, Dictionary<int, int>>? _birthdayIndex;

	/// <summary>
	/// Aim: Get all persons with birthdays in a specific month.
	/// </summary>
	/// <param name="month">Month (1-12).</param>
	/// <returns>List of persons with birthdays in that month.</returns>
	public static List<Person> GetByMonth(int month)
	{
		if (month < 1 || month > 12)
		{
			return [];
		}

		LoadMonthIndex(month);
		if (!_birthdayIndex!.TryGetValue(month, out var index))
		{
			return [];
		}

		var persons = new List<Person>();
		foreach (var personId in index.Values)
		{
			var person = GetPerson(personId);
			if (person != null)
			{
				persons.Add(person);
			}
		}
		return persons;
	}

	/// <summary>
	/// Aim: Get a person by id.
	/// </summary>
	/// <param name="id">The person id.</param>
	/// <returns>The person or null if not found.</returns>
	public static Person? GetPerson(int id)
	{
		var key = $"{MobileConstants.PREFS_PERSON_PREFIX}{id}";
		return PrefsHelper.GetValue<Person>(key);
	}

	/// <summary>
	/// Aim: Add a person to the birthday list.
	/// </summary>
	/// <param name="person">The person to add.</param>
	public static void Add(Person person)
	{
		SavePerson(person);

		if (person.Birthday == null)
		{
			return;
		}

		int month = person.Birthday.Month;
		LoadMonthIndex(month);

		if (!_birthdayIndex!.ContainsKey(month))
		{
			_birthdayIndex[month] = [];
		}

		_birthdayIndex[month][person.Id] = person.Id;
		SaveMonthIndex(month);
	}

	/// <summary>
	/// Aim: Update a person in the birthday list.
	/// </summary>
	/// <param name="person">The person to update.</param>
	public static void Update(Person person)
	{
		var oldPerson = GetPerson(person.Id);
		if (oldPerson?.Birthday?.Month != person.Birthday?.Month)
		{
			if (oldPerson?.Birthday != null)
			{
				RemoveFromMonthIndex(oldPerson.Birthday.Month, person.Id);
			}
			if (person.Birthday != null)
			{
				LoadMonthIndex(person.Birthday.Month);
				if (!_birthdayIndex!.ContainsKey(person.Birthday.Month))
				{
					_birthdayIndex[person.Birthday.Month] = [];
				}
				_birthdayIndex[person.Birthday.Month][person.Id] = person.Id;
				SaveMonthIndex(person.Birthday.Month);
			}
		}

		SavePerson(person);
	}

	/// <summary>
	/// Aim: Remove a person from the birthday list by id.
	/// </summary>
	/// <param name="id">The person id.</param>
	public static void Remove(int id)
	{
		var person = GetPerson(id);
		if (person?.Birthday != null)
		{
			RemoveFromMonthIndex(person.Birthday.Month, id);
		}

		var key = $"{MobileConstants.PREFS_PERSON_PREFIX}{id}";
		PrefsHelper.RemoveKey(key);
	}

	private static void SavePerson(Person person)
	{
		var key = $"{MobileConstants.PREFS_PERSON_PREFIX}{person.Id}";
		PrefsHelper.SetValue(key, person);
	}

	private static void RemoveFromMonthIndex(int month, int personId)
	{
		LoadMonthIndex(month);
		if (_birthdayIndex!.TryGetValue(month, out var index))
		{
			index.Remove(personId);
			SaveMonthIndex(month);
		}
	}

	private static void LoadMonthIndex(int month)
	{
		_birthdayIndex ??= [];

		if (_birthdayIndex.ContainsKey(month))
		{
			return;
		}

		var key = $"{MobileConstants.PREFS_BIRTHDAYS_PREFIX}{month}";
		var index = PrefsHelper.GetValue<Dictionary<int, int>>(key);
		_birthdayIndex[month] = index ?? [];
	}

	private static void SaveMonthIndex(int month)
	{
		var key = $"{MobileConstants.PREFS_BIRTHDAYS_PREFIX}{month}";
		if (_birthdayIndex!.TryGetValue(month, out var index))
		{
			PrefsHelper.SetValue(key, index);
		}
	}
}

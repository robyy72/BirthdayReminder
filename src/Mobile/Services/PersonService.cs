namespace Mobile;

/// <summary>
/// Aim: Service for managing persons list (uses App.Persons as cache).
/// </summary>
public static class PersonService
{
	public static void Load()
	{
		var persons = PrefsHelper.GetValue<List<Person>>(MobileConstants.PREFS_PERSONS);
		if (persons != null)
		{
			App.Persons = persons;
		}
		else
		{
			App.Persons = new List<Person>();
		}
    }

    /// <summary>
    /// Aim: Save App.Persons to prefs.
    /// </summary>
    public static void Save()
	{
		PrefsHelper.SetValue(MobileConstants.PREFS_PERSONS , App.Persons);
	}

	/// <summary>
	/// Aim: Add a person to App.Persons and save.
	/// Params: person - The person to add
	/// </summary>
	public static void Add(Person person)
	{
		person.Id = GetNextId();
		App.Persons.Add(person);
		Save();
	}

	/// <summary>
	/// Aim: Update a person in App.Persons and save.
	/// Params: person - The person to update
	/// </summary>
	public static void Update(Person person)
	{
		var existing = App.Persons.FirstOrDefault(p => p.Id == person.Id);
		if (existing != null)
		{
			int index = App.Persons.IndexOf(existing);
			App.Persons[index] = person;
			Save();
		}
	}

	/// <summary>
	/// Aim: Remove a person by id from App.Persons and save.
	/// Params: id - The id of the person to remove
	/// </summary>
	public static void Remove(int id)
	{
		var person = App.Persons.FirstOrDefault(p => p.Id == id);
		if (person != null)
		{
			App.Persons.Remove(person);
			Save();
		}
	}

	/// <summary>
	/// Aim: Get a person by id from App.Persons.
	/// Params: id - The id of the person
	/// Return: The person or null if not found
	/// </summary>
	public static Person? GetById(int id)
	{
		var person = App.Persons.FirstOrDefault(p => p.Id == id);
		return person;
	}

	/// <summary>
	/// Aim: Get the next available id.
	/// Return: The next id
	/// </summary>
	private static int GetNextId()
	{
		if (App.Persons.Count == 0)
		{
			return 1;
		}

		int maxId = App.Persons.Max(p => p.Id);
		int nextId = maxId + 1;
		return nextId;
	}
}

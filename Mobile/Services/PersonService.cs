namespace Mobile;

/// <summary>
/// Aim: Service for managing persons list.
/// </summary>
public static class PersonService
{
	private static List<Person>? _persons;
	private static bool _isLoaded;

	/// <summary>
	/// Aim: Get the current persons list, loading from prefs if needed (only once).
	/// </summary>
	/// <returns>The persons list.</returns>
	public static List<Person> Get()
	{
		if (_isLoaded && _persons != null)
		{
			return _persons;
		}

		_persons = PrefsHelper.GetValue<List<Person>>(MobileConstants.PREFS_PERSONS);
		if (_persons == null)
		{
			_persons = [];
		}

		_isLoaded = true;
		return _persons;
	}

	/// <summary>
	/// Aim: Save the current persons list to prefs.
	/// </summary>
	public static void Save()
	{
		if (_persons != null)
		{
			PrefsHelper.SetValue(MobileConstants.PREFS_PERSONS, _persons);
		}
	}

	/// <summary>
	/// Aim: Update persons list and save.
	/// </summary>
	/// <param name="persons">The persons list to save.</param>
	public static void Update(List<Person> persons)
	{
		_persons = persons;
		Save();
	}

	/// <summary>
	/// Aim: Add a person and save.
	/// </summary>
	/// <param name="person">The person to add.</param>
	public static void Add(Person person)
	{
		var persons = Get();
		person.Id = GetNextId();
		persons.Add(person);
		Save();
	}

	/// <summary>
	/// Aim: Remove a person by id and save.
	/// </summary>
	/// <param name="id">The id of the person to remove.</param>
	public static void Remove(int id)
	{
		var persons = Get();
		var person = persons.FirstOrDefault(p => p.Id == id);
		if (person != null)
		{
			persons.Remove(person);
			Save();
		}
	}

	/// <summary>
	/// Aim: Get a person by id.
	/// </summary>
	/// <param name="id">The id of the person.</param>
	/// <returns>The person or null if not found.</returns>
	public static Person? GetById(int id)
	{
		var persons = Get();
		var person = persons.FirstOrDefault(p => p.Id == id);
		return person;
	}

	/// <summary>
	/// Aim: Get the next available id.
	/// </summary>
	/// <returns>The next id.</returns>
	private static int GetNextId()
	{
		var persons = Get();
		if (persons.Count == 0)
		{
			return 1;
		}

		int maxId = persons.Max(p => p.Id);
		int nextId = maxId + 1;
		return nextId;
	}

	/// <summary>
	/// Aim: Clear the cached persons (used when prefs are cleared).
	/// </summary>
	public static void ClearCache()
	{
		_persons = null;
		_isLoaded = false;
	}
}

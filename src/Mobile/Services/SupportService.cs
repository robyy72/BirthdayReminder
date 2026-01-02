#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for managing support entries list (uses App.SupportEntries as cache).
/// </summary>
public static class SupportService
{
	public static void Load()
	{
		var entries = PrefsHelper.GetValue<List<Support>>(MobileConstants.PREFS_SUPPORT);
		if (entries != null)
		{
			App.SupportEntries = entries;
		}
		else
		{
			App.SupportEntries = new List<Support>();
		}
	}

	/// <summary>
	/// Aim: Save App.SupportEntries to prefs.
	/// </summary>
	public static void Save()
	{
		PrefsHelper.SetValue(MobileConstants.PREFS_SUPPORT, App.SupportEntries);
	}

	/// <summary>
	/// Aim: Add a support entry to App.SupportEntries and save.
	/// Params: entry - The support entry to add
	/// </summary>
	public static void Add(Support entry)
	{
		entry.Id = GetNextId();
		entry.CreatedAt = DateTime.Now;
		App.SupportEntries.Add(entry);
		Save();
	}

	/// <summary>
	/// Aim: Update a support entry in App.SupportEntries and save.
	/// Params: entry - The support entry to update
	/// </summary>
	public static void Update(Support entry)
	{
		var existing = App.SupportEntries.FirstOrDefault(s => s.Id == entry.Id);
		if (existing != null)
		{
			int index = App.SupportEntries.IndexOf(existing);
			App.SupportEntries[index] = entry;
			Save();
		}
	}

	/// <summary>
	/// Aim: Remove a support entry by id from App.SupportEntries and save.
	/// Params: id - The id of the support entry to remove
	/// </summary>
	public static void Remove(int id)
	{
		var entry = App.SupportEntries.FirstOrDefault(s => s.Id == id);
		if (entry != null)
		{
			App.SupportEntries.Remove(entry);
			Save();
		}
	}

	/// <summary>
	/// Aim: Get a support entry by id from App.SupportEntries.
	/// Params: id - The id of the support entry
	/// Return: The support entry or null if not found
	/// </summary>
	public static Support? GetById(int id)
	{
		var entry = App.SupportEntries.FirstOrDefault(s => s.Id == id);
		return entry;
	}

	/// <summary>
	/// Aim: Get all support entries filtered by type.
	/// Params: type - The SupportType to filter by
	/// Return: List of support entries matching the type
	/// </summary>
	public static List<Support> GetByType(SupportType type)
	{
		var entries = App.SupportEntries
			.Where(s => s.Type == (int)type)
			.OrderByDescending(s => s.CreatedAt)
			.ToList();
		return entries;
	}

	/// <summary>
	/// Aim: Get all support entries.
	/// Return: List of all support entries ordered by creation date
	/// </summary>
	public static List<Support> GetAll()
	{
		var entries = App.SupportEntries
			.OrderByDescending(s => s.CreatedAt)
			.ToList();
		return entries;
	}

	/// <summary>
	/// Aim: Get the next available id for a support entry.
	/// Return: The next available id
	/// </summary>
	private static int GetNextId()
	{
		if (App.SupportEntries.Count == 0)
		{
			return 1;
		}

		var maxId = App.SupportEntries.Max(s => s.Id);
		return maxId + 1;
	}
}

	namespace Mobile;

/// <summary>
/// Aim: Represents a complete reminder configuration.
/// Contains the days before the birthday and all enabled methods.
/// </summary>
public class Reminder
{
	public int DaysBefore { get; set; }
	public List<ReminderMethodLocal> LocalMethods { get; set; } = [];
	public List<ReminderMethodExternal> ExternalMethods { get; set; } = [];

	#region Helper Properties
	public bool HasAnyMethodEnabled =>
		LocalMethods.Any(m => m.Enabled) ||
		ExternalMethods.Any(m => m.Enabled);

	public int EnabledMethodCount =>
		LocalMethods.Count(m => m.Enabled) +
		ExternalMethods.Count(m => m.Enabled);
	#endregion
}

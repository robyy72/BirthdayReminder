namespace Mobile;

/// <summary>
/// Aim: Base class for all reminder methods (local and external).
/// </summary>
public abstract class ReminderMethodBase
{
	public bool Enabled { get; set; }
	public int TimeMinutes { get; set; }
}

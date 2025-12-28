#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a local reminder method (Notification or Alarm).
/// These are handled entirely on the device without server interaction.
/// </summary>
public class ReminderMethodLocal : ReminderMethodBase
{
	public LocalMethodType Type { get; set; }

	#region Sound & Vibration
	public bool PlaySound { get; set; }
	public bool Vibrate { get; set; }
	public string? CustomSoundFile { get; set; }
	#endregion

	#region Alarm-specific
	public bool WakeScreen { get; set; }
	public bool OverrideSilentMode { get; set; }
	public NotificationPriority Priority { get; set; }
	#endregion
}

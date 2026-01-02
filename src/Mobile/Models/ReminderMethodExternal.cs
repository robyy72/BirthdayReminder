#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents an external reminder method (Email, SMS, WhatsApp, Signal).
/// These open another app with a pre-filled message template.
/// </summary>
public class ReminderMethodExternal : ReminderMethodBase
{
	public ExternalMethodType Type { get; set; }

	#region Message Options
	/// <summary>
	/// Custom template (if null, default from Resources is used).
	/// </summary>
	public string? CustomMessageTemplate { get; set; }
	public bool IncludeAge { get; set; }
	#endregion
}

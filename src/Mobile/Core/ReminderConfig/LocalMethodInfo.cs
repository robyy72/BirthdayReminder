#region Usings
using Common;
using MobileLanguages;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Configuration info for a local reminder method.
/// </summary>
public record LocalMethodInfo(
	bool IsFree,
	int DefaultTime,
	string Icon,
	string DisplayNameKey,
	NotificationPriority DefaultPriority,
	bool DefaultSound,
	bool DefaultVibrate,
	bool DefaultWakeScreen,
	bool DefaultOverrideSilentMode
)
{
	public string DisplayName => Resources.ResourceManager.GetString(DisplayNameKey) ?? DisplayNameKey;
}

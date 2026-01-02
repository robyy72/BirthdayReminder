#region Usings
using MobileLanguages;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Configuration info for an external reminder method.
/// </summary>
public record ExternalMethodInfo(
	bool IsFree,
	int DefaultTime,
	string Icon,
	string DisplayNameKey,
	string TemplateKeyToday,
	string TemplateKeyTomorrow,
	string TemplateKeyInDays
)
{
	public string DisplayName =>
		Resources.ResourceManager.GetString(DisplayNameKey) ?? DisplayNameKey;

	public string GetTemplate(int daysUntilBirthday) => daysUntilBirthday switch
	{
		0 => Resources.ResourceManager.GetString(TemplateKeyToday) ?? TemplateKeyToday,
		1 => Resources.ResourceManager.GetString(TemplateKeyTomorrow) ?? TemplateKeyTomorrow,
		_ => Resources.ResourceManager.GetString(TemplateKeyInDays) ?? TemplateKeyInDays
	};
}

#region Usings
using System.Text.Json.Serialization;
#endregion

namespace Mobile;

/// <summary>
/// Aim: JSON model for timezone data from timezones.json.
/// </summary>
public class TimezoneJsonEntry
{
	[JsonPropertyName("offset")]
	public double Offset { get; set; }

	[JsonPropertyName("region")]
	public string Region { get; set; } = string.Empty;

	[JsonPropertyName("city")]
	public string City { get; set; } = string.Empty;

	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;
}

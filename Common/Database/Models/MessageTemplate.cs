#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace Common;

/// <summary>
/// Aim: Base message template for customer communication.
/// </summary>
public abstract class MessageTemplate
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(100)]
	public string Name { get; set; } = string.Empty;

	[Required]
	public string Content { get; set; } = string.Empty;

	public bool IsActive { get; set; } = true;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Aim: Email message template.
/// </summary>
public class EmailText : MessageTemplate
{
	[Required]
	[MaxLength(200)]
	public string Subject { get; set; } = string.Empty;

	public bool IsHtml { get; set; } = true;
}

/// <summary>
/// Aim: Messenger text template (WhatsApp, Signal, SMS).
/// </summary>
public class MessengerText : MessageTemplate
{
	public PreferredChannel Channel { get; set; } = PreferredChannel.WhatsApp;

	[MaxLength(500)]
	public string? PreviewText { get; set; }
}

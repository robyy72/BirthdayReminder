#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace Common;

/// <summary>
/// Aim: Admin user entity for backend login.
/// </summary>
public class SystemUser
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(200)]
	public string Email { get; set; } = string.Empty;

	[Required]
	public string PasswordHash { get; set; } = string.Empty;

	[MaxLength(100)]
	public string? DisplayName { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset? LastLoginAt { get; set; }

	public bool IsActive { get; set; } = true;
}

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Admin user entity for backend login.
/// </summary>
public class SystemUser
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(100)]
	public string Username { get; set; } = string.Empty;

	[Required]
	public string PasswordHash { get; set; } = string.Empty;

	[MaxLength(200)]
	public string? Email { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime? LastLoginAt { get; set; }

	public bool IsActive { get; set; } = true;
}

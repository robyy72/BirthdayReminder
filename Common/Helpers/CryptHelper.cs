#region Usings
using BCrypt.Net;
#endregion

namespace Common;

/// <summary>
/// Aim: Helper class for cryptographic operations.
/// </summary>
public static class CryptHelper
{
	#region Public Methods
	/// <summary>
	/// Aim: Hash a password using BCrypt.
	/// Params: password - the plain text password.
	/// Return (string): The hashed password.
	/// </summary>
	public static string HashPassword(string password)
	{
		var hash = BCrypt.Net.BCrypt.HashPassword(password);
		return hash;
	}

	/// <summary>
	/// Aim: Verify a password against a hash.
	/// Params: password - the plain text password, hash - the stored hash.
	/// Return (bool): True if password matches, false otherwise.
	/// </summary>
	public static bool VerifyPassword(string password, string hash)
	{
		var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
		return isValid;
	}
	#endregion
}

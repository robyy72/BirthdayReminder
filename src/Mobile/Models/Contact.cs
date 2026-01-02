#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Represents a contact from the phone with reduced fields
/// </summary>
public class Contact
{
	#region Identity
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public Birthday? Birthday { get; set; }
	#endregion
}

#region Usings
using System.ComponentModel;
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: ViewModel for displaying a contact in sync list with selection.
/// </summary>
public class ContactSyncViewModel : INotifyPropertyChanged
{
	#region Private Fields
	private bool _isSelected;
	#endregion

	#region Properties
	public Contact Contact { get; }
	public string DisplayName { get; set; } = string.Empty;
	public string BirthdayDisplay { get; set; } = string.Empty;
	public bool HasBirthday { get; set; }

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
			}
		}
	}
	#endregion

	#region Events
	public event PropertyChangedEventHandler? PropertyChanged;
	#endregion

	#region Constructor
	public ContactSyncViewModel(Contact contact)
	{
		Contact = contact;
		DisplayName = !string.IsNullOrEmpty(contact.DisplayName)
			? contact.DisplayName
			: $"{contact.FirstName} {contact.LastName}".Trim();

		if (contact.Birthday != null)
		{
			HasBirthday = true;
			BirthdayDisplay = BirthdayHelper.GetDateDisplay(contact.Birthday);
		}
	}
	#endregion
}

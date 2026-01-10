#region Usings
using Common;
#endregion

namespace Mobile;

public partial class SyncContactsToPersonsPage : ContentPage
{
	#region Properties
	public List<ContactSyncViewModel> NewContacts { get; set; } = [];
	#endregion

	#region Constructor
	public SyncContactsToPersonsPage()
	{
		InitializeComponent();
		BindingContext = this;
		Init();
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Aim: Initialize the page with new contacts.
	/// </summary>
	private void Init()
	{
		var existingContactIds = App.Persons
			.Where(p => !string.IsNullOrEmpty(p.ContactId))
			.Select(p => p.ContactId)
			.ToHashSet();

		NewContacts = App.Contacts
			.Where(c => !existingContactIds.Contains(c.Id.ToString()))
			.Select(c => new ContactSyncViewModel(c))
			.ToList();

		CountLabel.Text = string.Format(
			MobileLanguages.Resources.Page_SyncContacts_Count,
			NewContacts.Count);
	}

	/// <summary>
	/// Aim: Handle select all checkbox change.
	/// Params: sender - The checkbox, e - Event args.
	/// </summary>
	private void OnSelectAllChanged(object? sender, CheckedChangedEventArgs e)
	{
		foreach (var contact in NewContacts)
		{
			contact.IsSelected = e.Value;
		}
	}

	/// <summary>
	/// Aim: Add selected contacts as persons and write back birthdays if allowed.
	/// </summary>
	private async void OnAddSelectedClicked(object? sender, EventArgs e)
	{
		var selectedContacts = NewContacts.Where(c => c.IsSelected).ToList();
		var contactsService = new ContactsService();

		foreach (var vm in selectedContacts)
		{
			Person person = ContactsService.ConvertContactToPerson(vm.Contact);
			person.Source = PersonSource.Contacts;
			PersonService.Add(person);

			// Write birthday back to contact if allowed and person has birthday
			if (person.Birthday != null && ContactsService.CanWriteToContacts())
			{
				await contactsService.UpdateContactBirthdayAsync(
					vm.Contact.Id.ToString(),
					person.Birthday);
			}
		}

		App.NeedsSyncContacts = false;
		NavigateToMain();
	}

	/// <summary>
	/// Aim: Skip sync and go to main page.
	/// </summary>
	private void OnSkipClicked(object? sender, EventArgs e)
	{
		App.NeedsSyncContacts = false;
		NavigateToMain();
	}

	/// <summary>
	/// Aim: Navigate to main page.
	/// </summary>
	private void NavigateToMain()
	{
		App.SetRootPage(App.CreateMainNavigationPage());
	}
	#endregion
}

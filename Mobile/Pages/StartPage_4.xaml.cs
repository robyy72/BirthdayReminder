namespace Mobile;

public partial class StartPage_4 : ContentPage
{
	private enum ResultState { Initial, Loading, Success, Denied }

	private CancellationTokenSource? _cts;
	private bool _isLoading;

	public StartPage_4()
	{
		InitializeComponent();
	}

	private async void OnRequestPermissionClicked(object? sender, EventArgs e)
	{
		bool granted = await DeviceService.RequestContactsReadPermissionAsync();

		if (granted)
		{
			await LoadContactsAsync();
		}
		else
		{
			ShowPanel(ResultState.Denied);
			App.Account.ContactsReadMode = ContactsReadMode.None;
			AccountService.Save();
			ButtonNext.IsEnabled = true;
		}
	}

	private async Task LoadContactsAsync()
	{
		_isLoading = true;
		_cts = new CancellationTokenSource();

		// Disable buttons during load
		ButtonRequestPermission.IsEnabled = false;
		ButtonNext.IsEnabled = false;

		ShowPanel(ResultState.Loading);

		try
		{
			var service = new ContactsService();
			bool onlyWithBirthday = App.Account.ContactsReadMode == ContactsReadMode.ReadNamesWithBirthday;
			var contacts = await service.GetContactsAsync(onlyWithBirthday);

			if (_cts.Token.IsCancellationRequested)
				return;

			// Store contacts in App
			foreach (var contact in contacts)
			{
				App.Persons.Add(contact);
			}

			// Build success message based on access level
			int totalCount = contacts.Count;
			int withBirthdayCount = contacts.Count(c => c.Birthday != null);
			var accessChoice = App.Account.ContactsAccessChoice;

			LabelSuccess.Text = accessChoice == ContactsAccessChoice.OnlySomeContacts
				? $"Eingeschränkter Zugriff erteilt, nur {totalCount} Kontakte eingelesen, davon {withBirthdayCount} mit Geburtstag"
				: $"Alle {totalCount} Kontakte erfolgreich eingelesen, davon {withBirthdayCount} mit Geburtstag";

			ShowPanel(ResultState.Success);
			ButtonNext.IsEnabled = true;
		}
		catch (OperationCanceledException)
		{
			// User navigated back, do nothing
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error loading contacts: {ex.Message}");
			LabelSuccess.Text = "Fehler beim Laden der Kontakte";
			ShowPanel(ResultState.Success);
		}
		finally
		{
			_isLoading = false;
			ButtonRequestPermission.IsEnabled = true;
			_cts?.Dispose();
			_cts = null;
		}
	}

	private void ShowPanel(ResultState state)
	{
		PanelInitial.IsVisible = state == ResultState.Initial;
		PanelLoading.IsVisible = state == ResultState.Loading;
		PanelSuccess.IsVisible = state == ResultState.Success;
		PanelDenied.IsVisible = state == ResultState.Denied;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (_isLoading)
		{
			_cts?.Cancel();
		}

		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_3();
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new StartPage_5();
		}
	}
}

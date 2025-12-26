namespace Mobile;

public partial class RequestPermissionPage_1 : ContentPage
{
	#region Fields
	private enum ResultState { Initial, Success, Denied }

	private readonly PermissionType _permissionType;
	#endregion

	#region Constructor
	public RequestPermissionPage_1(PermissionType permissionType)
	{
		InitializeComponent();
		_permissionType = permissionType;
	}
	#endregion

	#region Lifecycle
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Check if permission was granted in Settings
		bool granted;
		switch (_permissionType)
		{
			case PermissionType.Contacts:
				granted = await DeviceService.CheckContactsReadPermissionAsync();
				break;
			case PermissionType.Calendar:
				granted = await DeviceService.CheckCalendarReadPermissionAsync();
				break;
			default:
				granted = false;
				break;
		}

		if (granted)
		{
			App.NeedsReadContacts = true;
            SetPermissionStatus(AppPermissionStatus.Granted);
			ShowPanel(ResultState.Success);
			LabelSuccess.Text = "Berechtigung erteilt";
		}

		switch (_permissionType)
		{
			case PermissionType.Contacts:
				ButtonNext.IsEnabled = App.Account.ContactsPermission != AppPermissionStatus.NotSet;
				break;
			case PermissionType.Calendar:
				ButtonNext.IsEnabled = App.Account.CalendarPermission != AppPermissionStatus.NotSet;
				break;
		}
	}
	#endregion

	#region Event Handlers
	private async void OnRequestPermissionClicked(object? sender, EventArgs e)
	{
		// Check if permission was already denied → go to RequestPermissionPage_2
		AppPermissionStatus currentStatus;
		switch (_permissionType)
		{
			case PermissionType.Contacts:
				currentStatus = App.Account.ContactsPermission;
				break;
			case PermissionType.Calendar:
				currentStatus = App.Account.CalendarPermission;
				break;
			default:
				currentStatus = AppPermissionStatus.NotSet;
				break;
		}

		if (currentStatus == AppPermissionStatus.Denied)
		{
			if (Application.Current?.Windows.Count > 0)
			{
				Application.Current.Windows[0].Page = new RequestPermissionPage_2(_permissionType);
			}
			return;
		}

		// First run: request permission via dialog
		bool granted;
		switch (_permissionType)
		{
			case PermissionType.Contacts:
				granted = await DeviceService.RequestContactsReadPermissionAsync();
				break;
			case PermissionType.Calendar:
				granted = await DeviceService.RequestCalendarReadPermissionAsync();
				break;
			default:
				granted = false;
				break;
		}

		if (granted)
		{
			App.NeedsReadContacts = true;
			SetPermissionStatus(AppPermissionStatus.Granted);
			ShowPanel(ResultState.Success);
			LabelSuccess.Text = "Berechtigung erteilt";
			ButtonNext.IsEnabled = true;
		}
		else
		{
			SetPermissionStatus(AppPermissionStatus.Denied);
			ShowPanel(ResultState.Denied);

			if (_permissionType == PermissionType.Contacts)
			{
				App.Account.ContactsReadMode = ContactsReadMode.None;
			}

			AccountService.Save();
			ButtonNext.IsEnabled = true;
		}
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0 && App.BackwardPage != null)
		{
			Application.Current.Windows[0].Page = App.BackwardPage;
		}
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0 && App.ForwardPage != null)
		{
			Application.Current.Windows[0].Page = App.ForwardPage;
		}
	}
	#endregion

	#region Private Methods
	private void SetPermissionStatus(AppPermissionStatus status)
	{
		switch (_permissionType)
		{
			case PermissionType.Contacts:
				App.Account.ContactsPermission = status;
				break;
			case PermissionType.Calendar:
				App.Account.CalendarPermission = status;
				break;
		}
		AccountService.Save();
	}

	private void ShowPanel(ResultState state)
	{
		PanelInitial.IsVisible = state == ResultState.Initial;
		PanelSuccess.IsVisible = state == ResultState.Success;
		PanelDenied.IsVisible = state == ResultState.Denied;
	}
	#endregion
}

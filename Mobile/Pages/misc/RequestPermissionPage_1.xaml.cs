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

		if (App.BackwardPage == null)
			throw new InvalidOperationException("BackwardPage is null.");
		if (App.ForwardPage == null)
			throw new InvalidOperationException("ForwardPage is null.");
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
			App.SetRootPage(new RequestPermissionPage_2(_permissionType));
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
		}
		else
		{
			SetPermissionStatus(AppPermissionStatus.Denied);
			ShowPanel(ResultState.Denied);
			LabelSuccess.Text = "Berechtigung verweigert";

			if (_permissionType == PermissionType.Contacts)
			{
				if (App.Account.ContactsReadMode != ContactsReadMode.None)
				{
					App.Account.ContactsReadMode = ContactsReadMode.None;
					AccountService.Save();
				}
			}
		}

		ButtonNext.IsEnabled = true;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(App.BackwardPage!);
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		App.SetRootPage(App.ForwardPage!);
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

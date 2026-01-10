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

		if (App.BackwardPageType == null)
			throw new InvalidOperationException("BackwardPageType is null.");
		if (App.ForwardPageType == null)
			throw new InvalidOperationException("ForwardPageType is null.");
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
			await SetPermissionStatusAsync(AppPermissionStatus.Granted);
			ShowPanel(ResultState.Success);
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
			await App.PushPageAsync(new RequestPermissionPage_2());
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
			await SetPermissionStatusAsync(AppPermissionStatus.Granted);
			ShowPanel(ResultState.Success);
		}
		else
		{
			await SetPermissionStatusAsync(AppPermissionStatus.Denied);
			ShowPanel(ResultState.Denied);
		}

		ButtonNext.IsEnabled = true;
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		var page = (Page)Activator.CreateInstance(App.BackwardPageType!)!;
		App.SetRootPage(page);
	}

	private void OnNextClicked(object? sender, EventArgs e)
	{
		var page = (Page)Activator.CreateInstance(App.ForwardPageType!)!;
		App.SetRootPage(page);
	}
	#endregion

	#region Private Methods
	private async Task SetPermissionStatusAsync(AppPermissionStatus status)
	{
		switch (_permissionType)
		{
			case PermissionType.Contacts:
				await PermissionService.UpdateContactsPermissionAsync(status);
				break;
			case PermissionType.Calendar:
				PermissionService.UpdateCalendarPermission(status);
				break;
		}
	}

	private void ShowPanel(ResultState state)
	{
		PanelInitial.IsVisible = state == ResultState.Initial;
		PanelSuccess.IsVisible = state == ResultState.Success;
		PanelDenied.IsVisible = state == ResultState.Denied;
	}
	#endregion
}

namespace Mobile;

public partial class RequestPermissionPage_2 : ContentPage
{
	#region Fields
	private readonly PermissionType _permissionType;
	#endregion

	#region Constructor
	public RequestPermissionPage_2(PermissionType permissionType)
	{
		InitializeComponent();
		_permissionType = permissionType;
	}
	#endregion

	#region Event Handlers
	private void OnOpenSettingsClicked(object? sender, EventArgs e)
	{
		AppInfo.ShowSettingsUI();
	}

	private void OnBackClicked(object? sender, EventArgs e)
	{
		// Go back to RequestPermissionPage_1, keep same Forward/Backward pages
		App.SetRootPage(new RequestPermissionPage_1(_permissionType));
	}
	#endregion
}

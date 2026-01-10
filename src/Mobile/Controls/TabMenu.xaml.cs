#region Usings
#endregion

namespace Mobile;

/// <summary>
/// Aim: Tab menu control for bottom navigation.
/// </summary>
public partial class TabMenu : ContentView
{
	#region Private Fields
	private readonly Color _activeColor;
	private readonly Color _inactiveColor;
	#endregion

	#region Bindable Properties
	public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(
		nameof(ActiveTab),
		typeof(TabMenuItem),
		typeof(TabMenu),
		TabMenuItem.Home,
		propertyChanged: OnActiveTabChanged);
	#endregion

	#region Properties
	public TabMenuItem ActiveTab
	{
		get => (TabMenuItem)GetValue(ActiveTabProperty);
		set => SetValue(ActiveTabProperty, value);
	}
	#endregion

	#region Constructor
	public TabMenu()
	{
		InitializeComponent();
		_activeColor = ResourceHelper.GetThemedColor("Primary", "Primary");
		_inactiveColor = ResourceHelper.GetThemedColor("Gray500", "Gray400");
		UpdateActiveTab();
	}
	#endregion

	#region Property Changed Handlers
	private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is TabMenu tabMenu)
		{
			tabMenu.UpdateActiveTab();
		}
	}
	#endregion

	#region Private Methods
	private void UpdateActiveTab()
	{
		SetTabColor(IconHome, LabelHome, ActiveTab == TabMenuItem.Home);
		SetTabColor(IconList, LabelList, ActiveTab == TabMenuItem.List);
		SetTabColor(IconPerson, LabelPerson, ActiveTab == TabMenuItem.Person);
		SetTabColor(IconSettings, LabelSettings, ActiveTab == TabMenuItem.Settings);
	}

	private void SetTabColor(Label icon, Label label, bool isActive)
	{
		Color color = isActive ? _activeColor : _inactiveColor;
		icon.TextColor = color;
		label.TextColor = color;
	}
	#endregion

	#region Event Handlers
	private void OnHomeClicked(object? sender, EventArgs e)
	{
		if (ActiveTab == TabMenuItem.Home)
			return;

		App.SetRootPage(App.CreateMainNavigationPage());
	}

	private async void OnListClicked(object? sender, EventArgs e)
	{
		if (ActiveTab == TabMenuItem.List)
			return;

		await App.NavigateToAsync<AllBirthdaysPage>();
	}

	private async void OnPersonClicked(object? sender, EventArgs e)
	{
		if (ActiveTab == TabMenuItem.Person)
			return;

		await App.NavigateToAsync<AccountPage>();
	}

	private async void OnSettingsClicked(object? sender, EventArgs e)
	{
		if (ActiveTab == TabMenuItem.Settings)
			return;

		await App.NavigateToAsync<SettingsPage>();
	}
	#endregion
}

public enum TabMenuItem
{
	Home,
	List,
	Person,
	Settings
}

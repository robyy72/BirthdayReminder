#region Usings
using Common;
#endregion

namespace Mobile;

public partial class App : Application
{
    #region Private Fields
    private static INavigation? _navigation;
    #endregion

    #region Public Properties

    /// <summary>
    /// Aim: Get the current persons list (loads from prefs once, then cached).
    /// </summary>
    public static List<Person> Persons { get; set; } = [];
    public static List<Contact> Contacts { get; set; } = [];
    public static List<Support> SupportEntries { get; set; } = [];
    public static Account Account { get; set; } = new();
	public static bool NeedsReadContacts { get; set; } = false;

	#region Navigation (for reusable pages)
	public static Type? ForwardPageType { get; set; }
	public static Type? BackwardPageType { get; set; }
    #endregion

    #region Flyout Events
    public static event Action? FlyoutOpenRequested;
    public static event Action? FlyoutCloseRequested;
    public static event Action? ContextMenuOpenRequested;
    public static event Action? ContextMenuCloseRequested;
    #endregion

    #endregion

	public App()
	{
		InitializeComponent();
		Init();
	}

    #region Proctected Methods
    protected override Window CreateWindow(IActivationState? activationState)
	{
		Page page;

		if (AccountService.IsFirstRun())
		{
			page = CreateMainNavigationPage();
		}
		else
		{
			page = new StartPage_1();
		}

		Window window = new Window(page);
		return window;
	}
    #endregion

    #region Public Methods

    #region Main Navigation Setup
    /// <summary>
    /// Aim: Erstellt die Haupt-NavigationPage und initialisiert die Navigation
    /// Return: NavigationPage mit MainPage als Root
    /// </summary>
    public static NavigationPage CreateMainNavigationPage()
	{
		var mainPage = new MainPage();
		var navigationPage = new NavigationPage(mainPage)
		{
			BarBackgroundColor = Application.Current?.RequestedTheme == AppTheme.Dark
				? Color.FromArgb("#1C1C1E")  // Gray900
				: Color.FromArgb("#0066CC"), // Primary
			BarTextColor = Colors.White
		};

		_navigation = navigationPage.Navigation;

		return navigationPage;
	}
    #endregion

    #region Root Page Methods
    /// <summary>
    /// Aim: Sets the root page of the application window.
    /// Params: page - The page to set as root
    /// </summary>
    public static void SetRootPage(Page page)
    {
        Current!.Windows[0].Page = page;
    }

    /// <summary>
    /// Aim: Pushes a page onto the navigation stack.
    /// If not in a NavigationPage, wraps the current page first.
    /// Params: page - The page to push
    /// </summary>
    public static async Task PushPageAsync(Page page)
    {
        var window = Current!.Windows[0];
        if (window.Page is NavigationPage navPage)
        {
            await navPage.PushAsync(page);
        }
        else
        {
            // Wrap current page in NavigationPage and push
            var currentPage = window.Page;
            var navigationPage = new NavigationPage(currentPage);
            window.Page = navigationPage;
            await navigationPage.PushAsync(page);
        }
    }

    /// <summary>
    /// Aim: Pops the current page from the navigation stack.
    /// </summary>
    public static async Task PopPageAsync()
    {
        var window = Current!.Windows[0];
        if (window.Page is NavigationPage navPage)
        {
            await navPage.PopAsync();
        }
    }
    #endregion

    #region Stack Navigation Methods
    /// <summary>
    /// Aim: Navigiert zu einer neuen Page
    /// Params: TPage - Der Page-Typ
    /// </summary>
    public static async Task NavigateToAsync<TPage>() where TPage : Page, new()
    {
        if (_navigation == null)
            return;

        var page = new TPage();
        await _navigation.PushAsync(page);
    }

    /// <summary>
    /// Aim: Navigiert zu einer neuen Page mit Parameter
    /// Params: TPage - Der Page-Typ, parameter - Der Parameter für die Page
    /// </summary>
    public static async Task NavigateToAsync<TPage>(object parameter) where TPage : Page
    {
        if (_navigation == null)
            return;

        var page = (TPage?)Activator.CreateInstance(typeof(TPage), parameter);
        if (page != null)
        {
            await _navigation.PushAsync(page);
        }
    }

    /// <summary>
    /// Aim: Navigiert zu einer neuen Page mit zwei Parametern
    /// Params: TPage - Der Page-Typ, param1/param2 - Die Parameter für die Page
    /// </summary>
    public static async Task NavigateToAsync<TPage>(object param1, object param2) where TPage : Page
    {
        if (_navigation == null)
            return;

        var page = (TPage?)Activator.CreateInstance(typeof(TPage), param1, param2);
        if (page != null)
        {
            await _navigation.PushAsync(page);
        }
    }

    /// <summary>
    /// Aim: Navigiert zur vorherigen Page
    /// </summary>
    public static async Task GoBackAsync()
    {
        if (_navigation == null)
            return;

        await _navigation.PopAsync();
    }

    /// <summary>
    /// Aim: Navigiert zur Root-Page
    /// </summary>
    public static async Task NavigateToRootAsync()
    {
        if (_navigation == null)
            return;

        await _navigation.PopToRootAsync();
    }
    #endregion

    #region Flyout Methods
    /// <summary>
    /// Aim: Öffnet das Flyout-Menü
    /// </summary>
    public static void OpenFlyout()
    {
        FlyoutOpenRequested?.Invoke();
    }

    /// <summary>
    /// Aim: Schließt das Flyout-Menü
    /// </summary>
    public static void CloseFlyout()
    {
        FlyoutCloseRequested?.Invoke();
    }

    /// <summary>
    /// Aim: Öffnet das Context-Menü
    /// </summary>
    public static void OpenContextMenu()
    {
        ContextMenuOpenRequested?.Invoke();
    }

    /// <summary>
    /// Aim: Schließt das Context-Menü
    /// </summary>
    public static void CloseContextMenu()
    {
        ContextMenuCloseRequested?.Invoke();
    }
    #endregion

    #endregion

    #region Private Methods
    private void Init()
	{
        bool startAlwaysWithWelcome = MobileConstants.START_ALWAYS_WITH_WELCOME;
        if (startAlwaysWithWelcome)
            PrefsHelper.RemoveAllKeys();

        PersonService.Load();
		AccountService.Load();
		SupportService.Load();

        CheckTimeZone();

		if (AccountService.UseContacts())
			NeedsReadContacts = true;

        ApplyTheme();
		SendHeartbeatIfNeeded();
    }

	private async void SendHeartbeatIfNeeded()
	{
		// Send heartbeat at most once per day
		var lastHeartbeat = Account.LastHeartbeat;
		if (DateTime.UtcNow - lastHeartbeat < TimeSpan.FromDays(1))
		{
			return;
		}

		var apiService = new ApiService();
		await apiService.SendHeartbeatAsync();
	}

    private void CheckTimeZone()
    {
        if (string.IsNullOrEmpty(Account.TimeZoneId))
        {
            Account.TimeZoneId = DeviceService.GetTimeZoneId();
            AccountService.Save();
        }
    }

    private void ApplyTheme()
	{
		DeviceService.ApplyTheme(Account.Theme);
	}
    #endregion
}

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
        SetupGlobalExceptionHandlers();
        InitAsync();
        InitSync();
    }

    #region Protected Methods
    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page page = GetStartPage();
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
            BarBackgroundColor = ResourceHelper.GetThemedColor("Primary", "Gray900"),
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
            NavigationPage navigationPage = new NavigationPage(currentPage);
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

    /// <summary>
    /// Aim: Asynchrone Initialisierung - läuft im Hintergrund, darf die App nicht blockieren.
    /// </summary>
    private async void InitAsync()
    {
        try
        {
            // ErrorDatabase initialisieren
            await ErrorDatabase.InitAsync();

            // Ab hier ist ErrorService.Handle() verfügbar
            await ErrorService.SyncPendingAsync();

            // Weitere Background-Tasks
            SendHeartbeatIfNeeded();
            SetupConnectivitySync();
        }
        catch (Exception ex)
        {
            // Nur für die kurze Zeit bis ErrorDatabase bereit ist
            LogDebugInfo("InitAsync failed", ex);
        }
    }

    /// <summary>
    /// Aim: Synchrone Initialisierung - muss fertig sein bevor CreateWindow aufgerufen wird.
    /// </summary>
    private void InitSync()
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
    }

    private async void SendHeartbeatIfNeeded()
    {
        try
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
        catch (Exception ex)
        {
            // Hier ist ErrorDatabase schon bereit
            ErrorService.Handle(ex);
        }
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

    private void SetupGlobalExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            ErrorService.HandleFatal((Exception)e.ExceptionObject);
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            ErrorService.Handle(e.Exception);
            e.SetObserved();
        };

#if ANDROID
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
        {
            ErrorService.HandleFatal(e.Exception);
            e.Handled = true;
        };
#endif

#if IOS
        ObjCRuntime.Runtime.MarshalManagedException += (s, e) =>
        {
            ErrorService.HandleFatal(e.Exception);
        };
#endif
    }

    private void SetupConnectivitySync()
    {
        Connectivity.Current.ConnectivityChanged += async (s, e) =>
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await ErrorService.SyncPendingAsync();
                }
                catch (Exception ex)
                {
                    // ErrorDatabase ist hier definitiv bereit
                    ErrorService.Handle(ex);
                }
            }
        };
    }

    /// <summary>
    /// Aim: Determine the start page based on app state.
    /// Return (Page): StartPage_1 for new users, BrokenVersionPage if crashed, MainPage otherwise.
    /// </summary>
    private Page GetStartPage()
    {
        if (AccountService.IsFirstRun())
            return new StartPage_1();

        if (ErrorService.IsBrokenVersion())
            return new BrokenVersionPage();

        return CreateMainNavigationPage();
    }

    /// <summary>
    /// Aim: Debug-Logging für die kurze Zeit bis ErrorDatabase bereit ist.
    /// Params: message (string) - Die zu loggende Nachricht.
    /// </summary>
    private static void LogDebugInfo(string message)
    {
        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    /// <summary>
    /// Aim: Debug-Logging mit Exception-Details.
    /// Params: message (string) - Die zu loggende Nachricht, ex (Exception) - Die Exception.
    /// </summary>
    private static void LogDebugInfo(string message, Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        System.Diagnostics.Debug.WriteLine($"  Exception: {ex.GetType().Name} - {ex.Message}");
        if (!string.IsNullOrEmpty(ex.StackTrace))
            System.Diagnostics.Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
    }

    #endregion
}
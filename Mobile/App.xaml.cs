namespace Mobile;

public partial class App : Application
{
	public static bool NeedsReloadBirthdays { get; set; }
	public static bool UseContacts { get; set; }

	#region Reminder Templates (used during StartPage wizard)
	public static Reminder? Reminder_1_Template { get; set; }
	public static Reminder? Reminder_2_Template { get; set; }
	public static Reminder? Reminder_3_Template { get; set; }
	#endregion

	/// <summary>
	/// Aim: Get the current account (reads from prefs via AccountService, keeps in sync).
	/// </summary>
	public static Account Account => AccountService.Get();

	/// <summary>
	/// Aim: Get the current persons list (reads from prefs via PersonService, only once).
	/// </summary>
	public static List<Person> Persons => PersonService.Get();

	public App()
	{
		InitializeComponent();
		ApplyTheme();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Page page;

		if (AccountService.IsInitialized())
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

	/// <summary>
	/// Aim: Erstellt die Haupt-NavigationPage und initialisiert den NavigationService
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

		NavigationService.Initialize(navigationPage.Navigation);

		return navigationPage;
	}

	private void ApplyTheme()
	{
		var account = Account;
		DeviceService.ApplyTheme(account.Theme);
	}
}

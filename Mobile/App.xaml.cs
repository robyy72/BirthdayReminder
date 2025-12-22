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
		Page page = new StartPage_1();

		if (AccountService.IsInitialized())
			page = new AppShell();

		Window window = new Window(page);
		return window;
	}

	private void ApplyTheme()
	{
		var account = Account;
		DeviceService.ApplyTheme(account.Theme);
	}
}

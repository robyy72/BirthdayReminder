namespace Mobile;

public partial class SelectLanguagePage : ContentPage
{
	#region Fields
	private bool _isLoading = true;
	private readonly List<LanguageEntry> _languages =
	[
		new LanguageEntry { Code = "de", Name = "Deutsch", Description = "German" },
		new LanguageEntry { Code = "en", Name = "English", Description = "Englisch" }
	];
	#endregion

	#region Constructor
	public SelectLanguagePage()
	{
		InitializeComponent();
		TheHeader.BackClickedCommand = new Command(OnBackClicked);
		LoadLanguages();
		_isLoading = false;
	}
	#endregion

	#region Navigation
	private void OnBackClicked()
	{
		if (App.BackwardPageType != null)
		{
			var page = (Page)Activator.CreateInstance(App.BackwardPageType)!;
			App.SetRootPage(page);
		}
		else
		{
			App.SetRootPage(App.CreateMainNavigationPage());
		}
	}
	#endregion

	#region Load
	private void LoadLanguages()
	{
		string currentLocale = App.Account.Locale ?? "de";

		foreach (var lang in _languages)
		{
			var tile = CreateLanguageTile(lang, lang.Code == currentLocale);
			LanguageTilesContainer.Children.Add(tile);
		}
	}

	private Border CreateLanguageTile(LanguageEntry lang, bool isSelected)
	{
		var tile = new Border
		{
			Style = (Style)Application.Current!.Resources["Tile"]
		};

		var grid = new Grid
		{
			ColumnDefinitions =
			[
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			]
		};

		var textStack = new VerticalStackLayout
		{
			Spacing = 2,
			VerticalOptions = LayoutOptions.Center
		};

		var nameLabel = new Label
		{
			Text = lang.Name,
			Style = (Style)Application.Current!.Resources["LabelInfoOnCard"],
			FontAttributes = FontAttributes.Bold
		};

		var descriptionLabel = new Label
		{
			Text = lang.Description,
			Style = (Style)Application.Current!.Resources["LabelInfoOnCard"],
			TextColor = (Color)Application.Current!.Resources["Gray500"]
		};

		textStack.Children.Add(nameLabel);
		textStack.Children.Add(descriptionLabel);
		grid.SetColumn(textStack, 0);

		var radio = new RadioButton
		{
			IsChecked = isSelected,
			Value = lang.Code,
			Style = (Style)Application.Current!.Resources["RadioStandard"],
			VerticalOptions = LayoutOptions.Center
		};
		radio.CheckedChanged += OnLanguageRadioChanged;
		grid.SetColumn(radio, 1);

		grid.Children.Add(textStack);
		grid.Children.Add(radio);
		tile.Content = grid;

		return tile;
	}
	#endregion

	#region Events
	private void OnLanguageRadioChanged(object? sender, CheckedChangedEventArgs e)
	{
		if (_isLoading || !e.Value)
			return;

		if (sender is RadioButton radio && radio.Value is string locale)
		{
			App.Account.Locale = locale;
			AccountService.Save();

			var culture = new System.Globalization.CultureInfo(locale);
			System.Globalization.CultureInfo.CurrentCulture = culture;
			System.Globalization.CultureInfo.CurrentUICulture = culture;
			MobileLanguages.Resources.Culture = culture;

			// Navigate back to trigger reload with new language
			OnBackClicked();
		}
	}
	#endregion
}

public class LanguageEntry
{
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
}

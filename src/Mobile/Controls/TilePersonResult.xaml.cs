namespace Mobile;

/// <summary>
/// Aim: Reusable tile for displaying a person search result.
/// </summary>
public partial class TilePersonResult : ContentView
{
	#region Bindable Properties
	public static readonly BindableProperty DisplayNameProperty = BindableProperty.Create(
		nameof(DisplayName),
		typeof(string),
		typeof(TilePersonResult),
		string.Empty,
		propertyChanged: OnDisplayNameChanged);

	public static readonly BindableProperty DateDisplayProperty = BindableProperty.Create(
		nameof(DateDisplay),
		typeof(string),
		typeof(TilePersonResult),
		string.Empty,
		propertyChanged: OnDateDisplayChanged);

	public static readonly BindableProperty PersonIdProperty = BindableProperty.Create(
		nameof(PersonId),
		typeof(int),
		typeof(TilePersonResult),
		0);
	#endregion

	#region Properties
	public string DisplayName
	{
		get => (string)GetValue(DisplayNameProperty);
		set => SetValue(DisplayNameProperty, value);
	}

	public string DateDisplay
	{
		get => (string)GetValue(DateDisplayProperty);
		set => SetValue(DateDisplayProperty, value);
	}

	public int PersonId
	{
		get => (int)GetValue(PersonIdProperty);
		set => SetValue(PersonIdProperty, value);
	}
	#endregion

	#region Constructor
	public TilePersonResult()
	{
		InitializeComponent();
	}
	#endregion

	#region Property Changed Handlers
	private static void OnDisplayNameChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is TilePersonResult tile)
		{
			tile.NameLabel.Text = (string)newValue;
		}
	}

	private static void OnDateDisplayChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is TilePersonResult tile)
		{
			tile.DateLabel.Text = (string)newValue;
		}
	}
	#endregion

	#region Event Handlers
	private async void OnTileTapped(object? sender, TappedEventArgs e)
	{
		await App.NavigateToAsync<EditPersonPage>(PersonId);
	}
	#endregion
}

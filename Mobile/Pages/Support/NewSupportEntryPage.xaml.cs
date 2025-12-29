#region Usings
using Common;
#endregion

namespace Mobile;

public partial class NewSupportEntryPage : ContentPage
{
	#region Fields
	private readonly SupportType _supportType;
	#endregion

	#region Constructor
	public NewSupportEntryPage(SupportType supportType)
	{
		InitializeComponent();
		_supportType = supportType;
		SetupHeader();
	}
	#endregion

	#region Private Methods
	private void SetupHeader()
	{
		TypeHeaderLabel.Text = _supportType switch
		{
			SupportType.Bug => MobileLanguages.Resources.Support_Type_Bug,
			SupportType.FeatureRequest => MobileLanguages.Resources.Support_Type_FeatureRequest,
			SupportType.Feedback => MobileLanguages.Resources.Support_Type_Feedback,
			_ => MobileLanguages.Resources.Support_Type_Feedback
		};
	}
	#endregion

	#region Event Handlers
	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var title = TitleEntry.Text?.Trim() ?? string.Empty;
		var text = TextEditor.Text?.Trim() ?? string.Empty;

		if (string.IsNullOrEmpty(title))
		{
			ErrorLabel.Text = MobileLanguages.Resources.Error_NameRequired;
			ErrorBorder.IsVisible = true;
			return;
		}

		ErrorBorder.IsVisible = false;

		var support = new Support
		{
			Type = (int)_supportType,
			Title = title,
			Text = text
		};

		SupportService.Add(support);

		await App.GoBackAsync();
	}

	private void OnEntryFocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			var primary = (Color)Application.Current!.Resources["Primary"];
			outerBorder.Stroke = primary;
			outerBorder.StrokeThickness = 2;
		}
	}

	private void OnEntryUnfocused(object? sender, FocusEventArgs e)
	{
		if (sender is Entry entry && entry.Parent?.Parent is Border outerBorder)
		{
			var strokeColor = Application.Current!.RequestedTheme == AppTheme.Dark
				? (Color)Application.Current.Resources["Gray700"]
				: (Color)Application.Current.Resources["Gray300"];
			outerBorder.Stroke = strokeColor;
			outerBorder.StrokeThickness = 1;
		}
	}
	#endregion
}

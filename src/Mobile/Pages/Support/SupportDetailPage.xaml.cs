#region Usings
using Common;
#endregion

namespace Mobile;

public partial class SupportDetailPage : ContentPage
{
	#region Fields
	private readonly int _entryId;
	private Support? _entry;
	#endregion

	#region Constructor
	public SupportDetailPage(int entryId)
	{
		InitializeComponent();
		_entryId = entryId;
	}
	#endregion

	#region Lifecycle
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadEntry();
	}
	#endregion

	#region Private Methods
	private async Task LoadEntry()
	{
		_entry = SupportService.GetById(_entryId);
		if (_entry == null)
		{
			await App.GoBackAsync();
			return;
		}

		var typeText = ((SupportType)_entry.Type) switch
		{
			SupportType.Bug => MobileLanguages.Resources.Support_Type_Bug,
			SupportType.FeatureRequest => MobileLanguages.Resources.Support_Type_FeatureRequest,
			SupportType.Feedback => MobileLanguages.Resources.Support_Type_Feedback,
			_ => MobileLanguages.Resources.Support_Type_Feedback
		};

		TypeHeaderLabel.Text = typeText;
		TitleLabel.Text = _entry.Title;
		TextLabel.Text = _entry.Text;
	}
	#endregion

	#region Event Handlers
	private async void OnDeleteClicked(object? sender, EventArgs e)
	{
		if (_entry == null)
		{
			return;
		}

		SupportService.Remove(_entry.Id);
		await App.GoBackAsync();
	}
	#endregion
}

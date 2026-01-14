#region Usings
using Common;
#endregion

namespace Mobile;

public partial class SelectTicketTypePage : ContentPage
{
	#region Constructor
	public SelectTicketTypePage()
	{
		InitializeComponent();
	}
	#endregion

	#region Event Handlers
	private async void OnContinueClicked(object? sender, EventArgs e)
	{
		var ticketType = GetSelectedTicketType();
		await App.NavigateToAsync<CreateTicketPage>(ticketType);
	}
	#endregion

	#region Private Methods
	private TicketType GetSelectedTicketType()
	{
		if (RadioBug.IsChecked)
			return TicketType.Error;
		if (RadioFeature.IsChecked)
			return TicketType.FeatureRequest;
		return TicketType.CustomerFeedback;
	}
	#endregion
}

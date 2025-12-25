#region Usings
using Common;
#endregion

namespace Mobile;

public partial class AccountPage : ContentPage
{
	#region Constructor
	public AccountPage()
	{
		InitializeComponent();
		LoadAccount();
	}
	#endregion

	#region Load
	private void LoadAccount()
	{
		var account = App.Account;

		EmailLabel.Text = account.Email ?? MobileLanguages.Resources.Account_NoEmail;

		SubscriptionLabel.Text = account.Subscription switch
		{
			SubscriptionTier.Plus => "Plus",
			SubscriptionTier.Pro => "Pro",
			_ => "Free"
		};

		if (account.ValidUntil.HasValue)
		{
			ValidUntilLabel.Text = account.ValidUntil.Value.ToString("d");
		}
		else
		{
			ValidUntilLabel.Text = MobileLanguages.Resources.Account_Unlimited;
		}
	}
	#endregion
}

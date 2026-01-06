#region Usings

using Common;

#endregion

namespace WebsiteCustomer;

/// <summary>
/// Aim: Constants specific to the Customer website (landing page).
/// </summary>
public static class WebsiteCustomerConstants
{
	public const string SUBDOMAIN = CommonConstants.DOMAIN;

	public const string DEFAULT_LANGUAGE = "en";
	public static readonly string[] SUPPORTED_LANGUAGES = ["de", "en", "es", "fr", "pt"];
}

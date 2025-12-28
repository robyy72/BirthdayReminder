#region Usings
using System;
using System.Threading.Tasks;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Verarbeitet Deep Links und navigiert zur entsprechenden Seite
/// </summary>
public static class DeepLinkService
{
    #region Constants
    public const string Scheme = "birthdayreminder";
    #endregion

    #region Public Methods
    /// <summary>
    /// Aim: Verarbeitet eine Deep Link URL (birthdayreminder://123)
    /// Params: url - Die Deep Link URL mit Person-ID
    /// Return: True wenn Person gefunden und navigiert, sonst false (normaler Start)
    /// </summary>
    public static async Task<bool> HandleUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        try
        {
            var uri = new Uri(url);

            if (!uri.Scheme.Equals(Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // URL format: birthdayreminder://123
            string idString = uri.Host;

            if (!int.TryParse(idString, out int personId))
            {
                return false;
            }

            // Prüfe ob Person existiert
            var person = PersonService.GetById(personId);
            if (person == null)
            {
                // Person nicht gefunden → normaler Start
                return false;
            }

            // Person gefunden → zur Detail-Page navigieren
            await App.NavigateToRootAsync();
            await App.NavigateToAsync<DetailPersonPage>(personId);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeepLinkService error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Aim: Verarbeitet eine Deep Link URL aus einem Uri-Objekt
    /// Params: uri - Das Uri-Objekt
    /// Return: True wenn erfolgreich verarbeitet, sonst false
    /// </summary>
    public static async Task<bool> HandleUrl(Uri? uri)
    {
        if (uri == null)
        {
            return false;
        }

        string url = uri.ToString();
        bool result = await HandleUrl(url);
        return result;
    }
    #endregion
}

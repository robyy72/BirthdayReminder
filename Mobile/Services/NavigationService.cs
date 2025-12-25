#region Usings
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Zentrale Navigation für die App ohne Shell
/// </summary>
public static class NavigationService
{
    #region Private Fields
    private static INavigation? _navigation;
    private static bool _isFlyoutOpen;
    #endregion

    #region Events
    public static event Action? FlyoutOpenRequested;
    public static event Action? FlyoutCloseRequested;
    #endregion

    #region Properties
    public static bool IsFlyoutOpen
    {
        get => _isFlyoutOpen;
        private set => _isFlyoutOpen = value;
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Aim: Initialisiert den NavigationService mit der Navigation-Referenz
    /// Params: navigation - Die INavigation Instanz der NavigationPage
    /// </summary>
    public static void Initialize(INavigation navigation)
    {
        _navigation = navigation;
    }
    #endregion

    #region Navigation Methods
    /// <summary>
    /// Aim: Navigiert zu einer neuen Page
    /// Params: TPage - Der Page-Typ
    /// </summary>
    public static async Task NavigateTo<TPage>() where TPage : Page, new()
    {
        if (_navigation == null)
            return;

        CloseFlyout();
        var page = new TPage();
        await _navigation.PushAsync(page);
    }

    /// <summary>
    /// Aim: Navigiert zu einer neuen Page mit Parameter
    /// Params: TPage - Der Page-Typ, parameter - Der Parameter für die Page
    /// </summary>
    public static async Task NavigateTo<TPage>(object parameter) where TPage : Page
    {
        if (_navigation == null)
            return;

        CloseFlyout();
        var page = (TPage?)Activator.CreateInstance(typeof(TPage), parameter);
        if (page != null)
        {
            await _navigation.PushAsync(page);
        }
    }

    /// <summary>
    /// Aim: Navigiert zur vorherigen Page
    /// </summary>
    public static async Task GoBack()
    {
        if (_navigation == null)
            return;

        CloseFlyout();
        await _navigation.PopAsync();
    }

    /// <summary>
    /// Aim: Navigiert zur Root-Page
    /// </summary>
    public static async Task NavigateToRoot()
    {
        if (_navigation == null)
            return;

        CloseFlyout();
        await _navigation.PopToRootAsync();
    }
    #endregion

    #region Flyout Methods
    /// <summary>
    /// Aim: Öffnet das Flyout-Menü
    /// </summary>
    public static void OpenFlyout()
    {
        IsFlyoutOpen = true;
        FlyoutOpenRequested?.Invoke();
    }

    /// <summary>
    /// Aim: Schließt das Flyout-Menü
    /// </summary>
    public static void CloseFlyout()
    {
        IsFlyoutOpen = false;
        FlyoutCloseRequested?.Invoke();
    }

    /// <summary>
    /// Aim: Togglet das Flyout-Menü
    /// </summary>
    public static void ToggleFlyout()
    {
        if (IsFlyoutOpen)
        {
            CloseFlyout();
        }
        else
        {
            OpenFlyout();
        }
    }
    #endregion
}

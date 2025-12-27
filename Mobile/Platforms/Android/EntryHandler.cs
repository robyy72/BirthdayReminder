#region Usings
using Microsoft.Maui.Handlers;
#endregion

namespace Mobile;

public static class EntryHandlerConfiguration
{
    public static void ConfigureEntryHandler()
    {
        EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, view) =>
        {
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        });
    }
}

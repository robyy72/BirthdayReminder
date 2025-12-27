#region Usings
using Microsoft.Maui.Handlers;
using UIKit;
#endregion

namespace Mobile;

public static class EntryHandlerConfiguration
{
    public static void ConfigureEntryHandler()
    {
        EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, view) =>
        {
            handler.PlatformView.BorderStyle = UITextBorderStyle.None;
            handler.PlatformView.BackgroundColor = UIColor.Clear;
        });
    }
}

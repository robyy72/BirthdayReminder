#region Usings
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
#endregion

namespace Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if ANDROID || IOS
		EntryHandlerConfiguration.ConfigureEntryHandler();
		SwitchHandlerConfiguration.ConfigureSwitchHandler();
#endif

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddSingleton<IBillingService, FakeBillingService>();
#else
		builder.Services.AddSingleton<IBillingService, StoreBillingService>();
#endif

		return builder.Build();
	}
}

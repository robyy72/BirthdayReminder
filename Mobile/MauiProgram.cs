#region Usings
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
#endregion

namespace Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		var config = new ConfigurationBuilder()
			.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
			.Build();

		string? sentryDsn = config["Sentry:Dsn"];
		SentryService.Initialize(sentryDsn);

		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.UseSentry(options =>
			{
				if (!string.IsNullOrWhiteSpace(sentryDsn))
				{
					options.Dsn = sentryDsn;
				}
#if DEBUG
				options.Environment = "development";
#else
				options.Environment = "production";
#endif
			})
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

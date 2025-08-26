using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using YuGiOhResult.ViewModels;
using YuGiOhResult.Views;

namespace YuGiOhResult;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddTransient<MainPageViewModel>();
		builder.Services.AddTransient<DeckListViewModel>();
        builder.Services.AddTransient<DeckRegistrationViewModel>();
        builder.Services.AddTransient<ResultListViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DeckList>();
        builder.Services.AddTransient<DeckRegistration>();
        builder.Services.AddTransient<ResultList>();

        return builder.Build();
	}
}

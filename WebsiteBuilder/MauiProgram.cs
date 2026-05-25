using Microsoft.Extensions.Logging;
using WebsiteBuilder.Pages;
using WebsiteBuilder.Services;

namespace WebsiteBuilder;

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

        builder.Services.AddSingleton<WebsiteDataService>();
        builder.Services.AddTransient<PersonPage>();
        builder.Services.AddTransient<AboutMePage>();
        builder.Services.AddTransient<ContactPage>();
        builder.Services.AddTransient<ProjectsPage>();
        builder.Services.AddTransient<ResumePage>();
        builder.Services.AddTransient<SiteConfigPage>();
        builder.Services.AddTransient<VfsEditorPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

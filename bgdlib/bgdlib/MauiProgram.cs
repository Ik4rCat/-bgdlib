using Microsoft.Extensions.Logging;
using bgdlib.Services;
using bgdlib.ViewModels;
using bgdlib.Views;

namespace bgdlib;

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
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<RssService>();
        builder.Services.AddSingleton<NoteFileService>();

        // ViewModels
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<FavoritesViewModel>();
        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddTransient<NoteEditorViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<JobsViewModel>();
        builder.Services.AddTransient<DocsViewModel>();

        // Views
        builder.Services.AddTransient<FeedPage>();
        builder.Services.AddTransient<FavoritesPage>();
        builder.Services.AddTransient<NotesPage>();
        builder.Services.AddTransient<NoteEditorPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ArticlePage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<JobsPage>();
        builder.Services.AddTransient<DocsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

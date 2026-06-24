using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AuthPage),       typeof(AuthPage));
        Routing.RegisterRoute(nameof(ArticlePage),    typeof(ArticlePage));
        Routing.RegisterRoute(nameof(NoteEditorPage), typeof(NoteEditorPage));
        Routing.RegisterRoute(nameof(DocsPage),       typeof(DocsPage));
        Routing.RegisterRoute(nameof(NotesPage),      typeof(NotesPage));
        Routing.RegisterRoute(nameof(PostDetailPage), typeof(PostDetailPage));
        Routing.RegisterRoute(nameof(CreatePostPage), typeof(CreatePostPage));
        Routing.RegisterRoute(nameof(SettingsPage),   typeof(SettingsPage));
        Routing.RegisterRoute(nameof(FavoritesPage),  typeof(FavoritesPage));
        Routing.RegisterRoute(nameof(JobsPage),       typeof(JobsPage));

        UpdateTabTitles();
        LocalizationService.Instance.PropertyChanged += (_, _) => UpdateTabTitles();

        if (!Preferences.Default.Get("is_registered", false))
            Dispatcher.Dispatch(async () => await GoToAsync(nameof(AuthPage)));
    }

    private void UpdateTabTitles()
    {
        var L = LocalizationService.Instance;
        TabRss.Title     = L["Tab_Feed"];
        TabMain.Title    = L["Tab_Main"];
        TabSearch.Title  = L["Tab_Search"];
        TabProfile.Title = L["Tab_Profile"];
    }
}

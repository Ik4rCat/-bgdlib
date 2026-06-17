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

        UpdateTabTitles();
        LocalizationService.Instance.PropertyChanged += (_, _) => UpdateTabTitles();
    }

    private void UpdateTabTitles()
    {
        var L = LocalizationService.Instance;
        TabMain.Title    = L["Tab_Main"];
        TabRss.Title     = L["Tab_Feed"];
        TabFavs.Title    = L["Tab_Saved"];
        TabProfile.Title = L["Tab_Profile"];
    }
}

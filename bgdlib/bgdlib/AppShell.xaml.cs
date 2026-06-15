using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ArticlePage),    typeof(ArticlePage));
        Routing.RegisterRoute(nameof(NoteEditorPage), typeof(NoteEditorPage));
        Routing.RegisterRoute(nameof(JobsPage),       typeof(JobsPage));
        Routing.RegisterRoute(nameof(DocsPage),       typeof(DocsPage));
        Routing.RegisterRoute(nameof(NotesPage),      typeof(NotesPage));

        UpdateTabTitles();
        LocalizationService.Instance.PropertyChanged += (_, _) => UpdateTabTitles();
    }

    private void UpdateTabTitles()
    {
        var L = LocalizationService.Instance;
        TabFeed.Title    = L["Tab_Feed"];
        TabSearch.Title  = L["Tab_Search"];
        TabSaved.Title   = L["Tab_Saved"];
        TabProfile.Title = L["Tab_Profile"];
    }
}

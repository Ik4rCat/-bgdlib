using bgdlib.Views;

namespace bgdlib;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрация роутов для навигации push
        Routing.RegisterRoute(nameof(ArticlePage), typeof(ArticlePage));
        Routing.RegisterRoute(nameof(NoteEditorPage), typeof(NoteEditorPage));
    }
}

namespace bgdlib.Views;

public partial class ArticlePage : ContentPage
{
    public ArticlePage(ViewModels.ArticleViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

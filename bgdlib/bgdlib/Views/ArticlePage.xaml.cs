using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.Views;

[QueryProperty(nameof(Url), "Url")]
[QueryProperty(nameof(ArticleTitle), "Title")]
public partial class ArticlePage : ContentPage
{
    private readonly DatabaseService _db;
    public string Url { get; set; } = string.Empty;
    public string ArticleTitle { get; set; } = string.Empty;

    public ArticlePage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = ArticleTitle;
        ArticleWebView.Source = new UrlWebViewSource { Url = Url };
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        if (await _db.IsFavoriteAsync(Url))
        {
            await DisplayAlert("Уже в избранном", "Эта статья уже сохранена", "OK");
            return;
        }
        await _db.AddFavoriteAsync(new FavoriteItem { Title = ArticleTitle, Url = Url });
        await DisplayAlert("Сохранено", "Статья добавлена в избранное", "OK");
    }
}

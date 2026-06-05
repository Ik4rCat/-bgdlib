using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class FeedPage : ContentPage
{
    private readonly FeedViewModel _vm;

    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_vm.Items.Any())
            await _vm.LoadCommand.ExecuteAsync(null);
    }

    private async void OnArticleSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FeedItem item)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync(nameof(ArticlePage),
                new Dictionary<string, object> { ["Url"] = item.Url, ["Title"] = item.Title });
        }
    }
}

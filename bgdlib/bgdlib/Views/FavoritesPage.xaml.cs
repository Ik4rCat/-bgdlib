using bgdlib.Models;
using bgdlib.ViewModels;

namespace bgdlib.Views;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage(FavoritesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((FavoritesViewModel)BindingContext).LoadCommand.ExecuteAsync(null);
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FavoriteItem item)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync(nameof(ArticlePage),
                new Dictionary<string, object> { ["Url"] = item.Url, ["Title"] = item.Title });
        }
    }
}

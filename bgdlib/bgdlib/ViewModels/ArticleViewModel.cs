using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;
using System.Collections.ObjectModel;

namespace bgdlib.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class ArticleViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private FeedItem? _item;
    [ObservableProperty] private string _readTimeLabel = "";
    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<FeedItem> RelatedItems { get; } = [];

    public bool HasRelated => RelatedItems.Count > 0;

    public ArticleViewModel(DatabaseService db)
    {
        _db = db;
    }

    partial void OnItemChanged(FeedItem? value)
    {
        if (value == null) return;
        var words = (value.Description?.Split(' ')?.Length ?? 0) + (value.Title?.Split(' ')?.Length ?? 0);
        var minutes = Math.Max(1, words / 200);
        ReadTimeLabel = $"{minutes} мин чтения";
    }

    [RelayCommand]
    private async Task GoBack() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task Share()
    {
        if (Item?.Url is { } url)
            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(
                new Microsoft.Maui.ApplicationModel.DataTransfer.ShareTextRequest { Uri = url, Title = Item.Title });
    }

    [RelayCommand]
    private async Task OpenInBrowser()
    {
        if (Item?.Url is { } url && Uri.TryCreate(url, UriKind.Absolute, out var uri))
            await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(uri);
    }

    [RelayCommand]
    private void OpenRelated(FeedItem item)
    {
        // Navigation to related article handled by shell
    }

    [RelayCommand]
    private async Task ToggleFavorite()
    {
        if (Item == null) return;
        Item.IsFavorite = !Item.IsFavorite;
        OnPropertyChanged(nameof(Item));
        if (Item.IsFavorite)
            await _db.AddFavoriteAsync(new FavoriteItem { Title = Item.Title, Url = Item.Url });
        else
            await _db.RemoveFavoriteAsync(Item.Url);
    }
}

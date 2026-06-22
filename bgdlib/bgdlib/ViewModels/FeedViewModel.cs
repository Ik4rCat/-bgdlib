using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;
using bgdlib.Services;



namespace bgdlib.ViewModels;

public partial class FeedViewModel : ObservableObject
{
    private readonly RssService _rss;
    private readonly DatabaseService _db;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private string _selectedEngine = "ALL";
    [ObservableProperty] private string _selectedCategory = "ALL";

    public ObservableCollection<FeedItem> Items { get; } = [];
    public List<string> Engines { get; } = ["ALL", "Unity", "Unreal", "Godot", "Other"];
    public List<string> Categories { get; } = ["ALL", "news", "tutorial", "job", "postmortem", "tool", "docs"];

    private List<FeedItem> _allItems = [];
    private HashSet<string> _favoriteUrls = [];

    public FeedViewModel(RssService rss, DatabaseService db)
    {
        _rss = rss;
        _db = db;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        StatusText = "Загрузка ленты...";

        var L = LocalizationService.Instance;
        try
        {
            _allItems = await _db.GetFeedItemsAsync();
            var favs = await _db.GetFavoritesAsync();
            _favoriteUrls = favs.Select(f => f.Url).ToHashSet();
            foreach (var item in _allItems)
                item.IsFavorite = _favoriteUrls.Contains(item.Url);
            ApplyFilters();

            StatusText = L["Feed_Loading"];
            var fresh = await _rss.FetchAllAsync();
            await _db.SaveFeedItemsAsync(fresh);
            foreach (var item in fresh)
                item.IsFavorite = _favoriteUrls.Contains(item.Url);
            _allItems = fresh;
            ApplyFilters();
            StatusText = L.Format("Feed_Updated", fresh.Count);
        }
        catch
        {
            StatusText = _allItems.Count > 0 ? L["Feed_NoNetworkCache"] : L["Feed_NoNetwork"];
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void SetEngine(string engine) => SelectedEngine = engine;

    [RelayCommand]
    public void SetCategory(string category) => SelectedCategory = category;

    [RelayCommand]
    public async Task ToggleFavoriteAsync(FeedItem item)
    {
        if (item.IsFavorite)
        {
            await _db.RemoveFavoriteAsync(item.Url);
            _favoriteUrls.Remove(item.Url);
            item.IsFavorite = false;
        }
        else
        {
            await _db.AddFavoriteAsync(new FavoriteItem
            {
                Title = item.Title,
                Url = item.Url,
                Source = item.Source,
                ImageUrl = item.ImageUrl,
                Engine = item.Engine,
                Category = item.Category,
            });
            _favoriteUrls.Add(item.Url);
            item.IsFavorite = true;
        }
        ApplyFilters();
    }

    [RelayCommand]
    public async Task OpenArticleAsync(FeedItem item)
    {
        await Shell.Current.GoToAsync(nameof(bgdlib.Views.ArticlePage),
            new Dictionary<string, object> { ["Item"] = item });
    }

    partial void OnSelectedEngineChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = _allItems.AsEnumerable();
        if (SelectedEngine != "ALL")
            filtered = filtered.Where(x => x.Engine == SelectedEngine);
        if (SelectedCategory != "ALL")
            filtered = filtered.Where(x => x.Category == SelectedCategory);

        Items.Clear();
        foreach (var item in filtered)
            Items.Add(item);
    }
}

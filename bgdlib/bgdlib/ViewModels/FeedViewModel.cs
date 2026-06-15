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
    [ObservableProperty] private string _selectedEngine = "Все";
    [ObservableProperty] private string _selectedCategory = "Все";

    public ObservableCollection<FeedItem> Items { get; } = [];
    public List<string> Engines { get; } = ["Все", "Unity", "Unreal", "Godot", "Other"];
    public List<string> Categories { get; } = ["Все", "news", "tutorial", "job", "postmortem", "tool", "docs"];

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

        try
        {
            _allItems = await _db.GetFeedItemsAsync();
            var favs = await _db.GetFavoritesAsync();
            _favoriteUrls = favs.Select(f => f.Url).ToHashSet();
            foreach (var item in _allItems)
                item.IsFavorite = _favoriteUrls.Contains(item.Url);
            ApplyFilters();

            var progress = new Progress<string>(s => StatusText = s);
            var fresh = await _rss.FetchAllAsync(progress);
            await _db.SaveFeedItemsAsync(fresh);
            foreach (var item in fresh)
                item.IsFavorite = _favoriteUrls.Contains(item.Url);
            _allItems = fresh;
            ApplyFilters();
            StatusText = $"Обновлено: {fresh.Count} статей";
        }
        catch
        {
            StatusText = _allItems.Count > 0 ? "Нет сети — показан кэш" : "Нет сети";
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

    partial void OnSelectedEngineChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = _allItems.AsEnumerable();
        if (SelectedEngine != "Все")
            filtered = filtered.Where(x => x.Engine == SelectedEngine);
        if (SelectedCategory != "Все")
            filtered = filtered.Where(x => x.Category == SelectedCategory);

        Items.Clear();
        foreach (var item in filtered)
            Items.Add(item);
    }
}

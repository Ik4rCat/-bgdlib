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
    public List<string> Engines { get; } = ["All", "Unity", "Unreal", "Godot", "S&box", "Other"];
    public List<string> Categories { get; } = ["All", "news", "tutorial", "job", "postmortem", "tool"];

    private List<FeedItem> _allItems = [];

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

        // Сначала показываем кэш
        _allItems = await _db.GetFeedItemsAsync();
        ApplyFilters();

        // Потом обновляем из сети
        try
        {
            var progress = new Progress<string>(s => StatusText = s);
            var fresh = await _rss.FetchAllAsync(progress);
            await _db.SaveFeedItemsAsync(fresh);
            _allItems = fresh;
            ApplyFilters();
            StatusText = $"Обновлено: {fresh.Count} статей";
        }
        catch
        {
            StatusText = "Нет сети — показан кэш";
        }

        IsLoading = false;
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

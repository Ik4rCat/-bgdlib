using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public class CategoryTile
{
    public string Name { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Letter { get; set; } = string.Empty;
    public CategoryTile(string name, string filter, string color, string letter)
    {
        Name = name; Filter = filter; Color = color; Letter = letter;
    }
}

public partial class SearchViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<FeedItem> _allItems = [];

    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _showResults = false;

    public ObservableCollection<FeedItem> Results { get; } = [];

    public List<CategoryTile> CategoryGrid { get; } =
    [
        new("Unity",       "Unity",      "#a6e3a1", "U"),
        new("Godot",       "Godot",      "#89b4fa", "G"),
        new("Unreal",      "Unreal",     "#f38ba8", "UE"),
        new("Туториалы",   "tutorial",   "#f9e2af", "T"),
        new("Вакансии",    "job",        "#fab387", "J"),
        new("Инструменты", "tool",       "#cba6f7", "I"),
        new("Постмортемы", "postmortem", "#eba0ac", "P"),
        new("Документации","docs",       "#94e2d5", "D"),
    ];

    public List<string> PopularTags { get; } =
        ["C#", "Unity", "Godot", "Шейдеры", "AI / NPC", "Level Design",
         "Blueprints", "Паттерны", "Pixel Art", "GDScript"];

    public SearchViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allItems = await _db.GetFeedItemsAsync();
    }

    partial void OnSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ShowResults = false;
            Results.Clear();
            return;
        }
        PerformSearch(value);
        ShowResults = true;
    }

    private void PerformSearch(string query)
    {
        Results.Clear();
        foreach (var item in _allItems.Where(x =>
            x.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Source.Contains(query, StringComparison.OrdinalIgnoreCase)))
            Results.Add(item);
    }

    [RelayCommand]
    public void SelectCategory(string filter)
    {
        Results.Clear();
        foreach (var item in _allItems.Where(x =>
            x.Engine == filter || x.Category == filter))
            Results.Add(item);
        ShowResults = true;
    }

    [RelayCommand]
    public void SelectTag(string tag)
    {
        SearchQuery = tag;
    }
}

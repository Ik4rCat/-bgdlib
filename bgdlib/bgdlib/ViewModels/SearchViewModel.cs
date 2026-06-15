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
    public string Icon { get; set; } = string.Empty;

    public CategoryTile(string name, string filter, string color, string letter, string icon = "")
    {
        Name = name; Filter = filter; Color = color; Letter = letter; Icon = icon;
    }
}

public partial class SearchViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<FeedItem> _allItems = [];

    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _showResults = false;

    public ObservableCollection<FeedItem> Results { get; } = [];
    public ObservableCollection<CategoryTile> CategoryGrid { get; } = [];
    public ObservableCollection<string> PopularTags { get; } = [];

    public SearchViewModel(DatabaseService db) => _db = db;

    public void RebuildLocalized()
    {
        var L = LocalizationService.Instance;
        CategoryGrid.Clear();
        CategoryGrid.Add(new("Unity",             "Unity",      "#a6e3a1", "U",  "")); // videogame_asset
        CategoryGrid.Add(new("Godot",             "Godot",      "#89b4fa", "G",  "")); // extension
        CategoryGrid.Add(new("Unreal",            "Unreal",     "#f38ba8", "UE", "")); // flash_on
        CategoryGrid.Add(new(L["Cat_Tutorials"],  "tutorial",   "#f9e2af", "T",  "")); // school
        CategoryGrid.Add(new(L["Cat_Jobs"],       "job",        "#fab387", "J",  "")); // work
        CategoryGrid.Add(new(L["Cat_Tools"],      "tool",       "#cba6f7", "I",  "")); // build
        CategoryGrid.Add(new(L["Cat_Postmortems"],"postmortem", "#eba0ac", "P",  "")); // book
        CategoryGrid.Add(new(L["Cat_Docs"],       "docs",       "#94e2d5", "D",  "")); // code

        PopularTags.Clear();
        foreach (var t in L.GetTags()) PopularTags.Add(t);
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allItems = await _db.GetFeedItemsAsync();
        if (CategoryGrid.Count == 0) RebuildLocalized();
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
    public void SelectTag(string tag) => SearchQuery = tag;
}

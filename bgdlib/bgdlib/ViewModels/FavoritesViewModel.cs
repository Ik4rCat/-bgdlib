using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class FavoritesViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _selectedEngine = "ALL";

    public List<string> EngineFilters { get; } = ["ALL", "Unity", "Godot", "Unreal", "Other"];
    public ObservableCollection<FavoriteItem> Items { get; } = [];

    private List<FavoriteItem> _allItems = [];

    public FavoritesViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allItems = await _db.GetFavoritesAsync();
        ApplyFilter();
    }

    [RelayCommand]
    public void SetEngine(string engine)
    {
        SelectedEngine = engine;
        ApplyFilter();
    }

    [RelayCommand]
    public async Task RemoveAsync(FavoriteItem item)
    {
        await _db.RemoveFavoriteAsync(item.Url);
        _allItems.Remove(item);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = SelectedEngine == "ALL"
            ? _allItems
            : _allItems.Where(x => x.Engine == SelectedEngine).ToList();

        Items.Clear();
        foreach (var item in filtered) Items.Add(item);
        IsEmpty = Items.Count == 0;
    }
}

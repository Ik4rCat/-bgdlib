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

    public ObservableCollection<FavoriteItem> Items { get; } = [];

    public FavoritesViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        var items = await _db.GetFavoritesAsync();
        Items.Clear();
        foreach (var item in items) Items.Add(item);
        IsEmpty = Items.Count == 0;
    }

    [RelayCommand]
    public async Task RemoveAsync(FavoriteItem item)
    {
        await _db.RemoveFavoriteAsync(item.Url);
        Items.Remove(item);
        IsEmpty = Items.Count == 0;
    }
}

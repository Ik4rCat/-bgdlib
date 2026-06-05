using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private bool _isGuest = true;
    [ObservableProperty] private string _displayName = "Гость";
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _cacheSize = "0 KB";

    public ProfileViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        var session = await _db.GetSessionAsync();
        IsGuest = session.IsGuest;
        DisplayName = session.IsGuest ? "Гость" : session.DisplayName;
        Email = session.Email;
        await RefreshCacheSizeAsync();
    }

    [RelayCommand]
    public async Task ClearCacheAsync()
    {
        await _db.ClearFeedCacheAsync();
        await RefreshCacheSizeAsync();
        await Shell.Current.DisplayAlert("Кэш очищен", "Статьи ленты удалены из кэша", "OK");
    }

    private async Task RefreshCacheSizeAsync()
    {
        var bytes = await _db.GetCacheSizeBytesAsync();
        CacheSize = bytes < 1024 * 1024
            ? $"{bytes / 1024.0:F1} KB"
            : $"{bytes / 1024.0 / 1024.0:F1} MB";
    }
}

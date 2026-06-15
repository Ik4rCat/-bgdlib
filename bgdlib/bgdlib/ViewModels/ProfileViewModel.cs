using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private bool _isGuest = true;
    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _cacheSize = "0 KB";
    [ObservableProperty] private string _currentLang = LocalizationService.Instance.CurrentLanguage;

    public ProfileViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        var session = await _db.GetSessionAsync();
        IsGuest = session.IsGuest;
        DisplayName = session.IsGuest ? string.Empty : session.DisplayName;
        Email = session.Email;
        CurrentLang = LocalizationService.Instance.CurrentLanguage;
        await RefreshCacheSizeAsync();
    }

    [RelayCommand]
    public async Task ClearCacheAsync()
    {
        var L = LocalizationService.Instance;
        await _db.ClearFeedCacheAsync();
        await RefreshCacheSizeAsync();
        await Shell.Current.DisplayAlertAsync(L["Profile_CacheCleared"], L["Profile_CacheClearedMsg"], L["OK"]);
    }

    [RelayCommand]
    public void SetLanguage(string lang)
    {
        LocalizationService.Instance.SetLanguage(lang);
        CurrentLang = lang;
    }

    [RelayCommand]
    public async Task GoToJobs() => await Shell.Current.GoToAsync(nameof(JobsPage));

    [RelayCommand]
    public async Task GoToDocs() => await Shell.Current.GoToAsync(nameof(DocsPage));

    [RelayCommand]
    public async Task GoToNotes() => await Shell.Current.GoToAsync(nameof(NotesPage));

    private async Task RefreshCacheSizeAsync()
    {
        var bytes = await _db.GetCacheSizeBytesAsync();
        CacheSize = bytes < 1024 * 1024
            ? $"{bytes / 1024.0:F1} KB"
            : $"{bytes / 1024.0 / 1024.0:F1} MB";
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly ApiService _api;

    [ObservableProperty] private bool _isGuest = true;
    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _cacheSize = "0 KB";
    [ObservableProperty] private string _currentLang = LocalizationService.Instance.CurrentLanguage;

    public string Initials => string.IsNullOrEmpty(DisplayName)
        ? "?"
        : string.Concat(DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(w => char.ToUpper(w[0]).ToString()));

    public ProfileViewModel(DatabaseService db, ApiService api) { _db = db; _api = api; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var isGuest = Preferences.Default.Get("is_guest",
            string.IsNullOrEmpty(Preferences.Default.Get("username", "")));
        IsGuest = isGuest;
        if (!isGuest)
        {
            DisplayName = Preferences.Default.Get("username", "");
            Email = Preferences.Default.Get("user_email", "");

            // Initialize join date on first profile load
            var joinDateStr = Preferences.Default.Get("join_date", "");
            if (string.IsNullOrEmpty(joinDateStr))
            {
                joinDateStr = DateTime.Now.ToString("O");
                Preferences.Default.Set("join_date", joinDateStr);
            }
            JoinDate = DateTime.TryParse(joinDateStr, null,
                System.Globalization.DateTimeStyles.RoundtripKind, out var dt)
                ? "С " + dt.ToString("MMMM yyyy")
                : string.Empty;

            // Avatar from local storage
            var avatarPath = Preferences.Default.Get("avatar_path", "");
            AvatarSource = !string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath)
                ? ImageSource.FromFile(avatarPath)
                : null;

            // Saved articles count
            var favs = await _db.GetFavoritesAsync();
            SavedCount = favs.Count;
        }
        else
        {
            DisplayName = string.Empty;
            Email = string.Empty;
            JoinDate = string.Empty;
            AvatarSource = null;
            SavedCount = 0;
        }
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
    public async Task LoginAsync() => await Shell.Current.GoToAsync(nameof(AuthPage));

    [RelayCommand]
    public async Task GoToSettingsAsync() => await Shell.Current.GoToAsync(nameof(SettingsPage));

    [RelayCommand]
    public async Task LogoutAsync()
    {
        var session = await _db.GetSessionAsync();
        if (!session.IsGuest)
            await _api.LogoutAsync(session.RefreshToken);
        await _db.ClearSessionAsync();
        IsGuest = true;
        DisplayName = string.Empty;
        Email = string.Empty;
    }

    [RelayCommand]
    public async Task GoToDocs() => await Shell.Current.GoToAsync(nameof(DocsPage));

    [RelayCommand]
    public async Task GoToNotes() => await Shell.Current.GoToAsync(nameof(NotesPage));

    [RelayCommand]
    public async Task GoToFavorites() => await Shell.Current.GoToAsync(nameof(FavoritesPage));

    partial void OnDisplayNameChanged(string value) => OnPropertyChanged(nameof(Initials));

    private async Task RefreshCacheSizeAsync()
    {
        var bytes = await _db.GetCacheSizeBytesAsync();
        CacheSize = bytes < 1024 * 1024
            ? $"{bytes / 1024.0:F1} KB"
            : $"{bytes / 1024.0 / 1024.0:F1} MB";
    }
}

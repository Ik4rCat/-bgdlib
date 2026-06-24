using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly ApiService _api;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _initials = string.Empty;

    public string AppVersion => AppInfo.Current.VersionString;
    public string BuildNumber => AppInfo.Current.BuildString;

    [ObservableProperty] private string _selectedAccent = "#E53935";
    [ObservableProperty] private string _selectedTheme = "dark";

    [ObservableProperty] private bool _notifEnabled = true;
    [ObservableProperty] private bool _autoplay = false;
    [ObservableProperty] private bool _dataSave = false;
    [ObservableProperty] private bool _reduceAnim = false;
    [ObservableProperty] private bool _showNsfw = false;

    partial void OnSelectedAccentChanged(string value) => Preferences.Default.Set("accent_color", value);
    partial void OnSelectedThemeChanged(string value)  => Preferences.Default.Set("theme", value);
    partial void OnNotifEnabledChanged(bool value)     => Preferences.Default.Set("notif_enabled", value);
    partial void OnAutoplayChanged(bool value)          => Preferences.Default.Set("autoplay", value);
    partial void OnDataSaveChanged(bool value)          => Preferences.Default.Set("data_save", value);
    partial void OnReduceAnimChanged(bool value)        => Preferences.Default.Set("reduce_anim", value);
    partial void OnShowNsfwChanged(bool value)          => Preferences.Default.Set("show_nsfw", value);

    public SettingsViewModel(DatabaseService db, ApiService api)
    {
        _db = db;
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var session = await _db.GetSessionAsync();
        Username = session.IsGuest ? "Гость" : session.DisplayName;
        Email    = session.IsGuest ? ""      : session.Email;
        Initials = GetInitials(Username);

        SelectedAccent = Preferences.Default.Get("accent_color", "#E53935");
        SelectedTheme  = Preferences.Default.Get("theme",        "dark");
        NotifEnabled   = Preferences.Default.Get("notif_enabled", true);
        Autoplay       = Preferences.Default.Get("autoplay",      false);
        DataSave       = Preferences.Default.Get("data_save",     false);
        ReduceAnim     = Preferences.Default.Get("reduce_anim",   false);
        ShowNsfw       = Preferences.Default.Get("show_nsfw",     false);
    }

    [RelayCommand]
    private void SetAccent(string hex) => SelectedAccent = hex;

    [RelayCommand]
    private void SetTheme(string theme) => SelectedTheme = theme;

    [RelayCommand]
    public async Task LogoutAsync()
    {
        var session = await _db.GetSessionAsync();
        if (!session.IsGuest)
            await _api.LogoutAsync(session.RefreshToken);
        await _db.ClearSessionAsync();
        Preferences.Default.Set("is_registered", false);
        Preferences.Default.Remove("username");
        Preferences.Default.Remove("user_email");
        Preferences.Default.Remove("is_guest");
        await Shell.Current.GoToAsync(nameof(AuthPage));
    }

    [RelayCommand]
    public async Task DeleteAccountAsync()
    {
        bool confirmed = await Shell.Current.DisplayAlert(
            "Удалить аккаунт",
            "Все данные будут удалены безвозвратно.",
            "Удалить", "Отмена");
        if (!confirmed) return;

        var session = await _db.GetSessionAsync();
        if (!session.IsGuest)
            await _api.LogoutAsync(session.RefreshToken);
        await _db.ClearSessionAsync();
        Preferences.Default.Clear();
        await Shell.Current.GoToAsync(nameof(AuthPage));
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
            : name[0].ToString().ToUpper();
    }
}

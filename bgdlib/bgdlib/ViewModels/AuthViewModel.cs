using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly DatabaseService _db;
    private readonly LocalizationService _loc = LocalizationService.Instance;

    [ObservableProperty] private bool _isLogin = true;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _isBusy;

    public string SubmitLabel => IsLogin ? _loc["Auth_Login"] : _loc["Auth_Register"];

    partial void OnIsLoginChanged(bool _) => OnPropertyChanged(nameof(SubmitLabel));

    public AuthViewModel(ApiService api, DatabaseService db)
    {
        _api = api;
        _db = db;
    }

    [RelayCommand] public void SwitchToLogin() => IsLogin = true;
    [RelayCommand] public void SwitchToRegister() => IsLogin = false;

    [RelayCommand]
    public async Task SubmitAsync()
    {
        HasError = false;
        IsBusy = true;
        try
        {
            AuthResponse? auth;
            if (IsLogin)
            {
                auth = await _api.LoginAsync(Email, Password);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    ShowError(_loc["Auth_Error"]);
                    return;
                }
                auth = await _api.RegisterAsync(Email, Name, Password);
            }

            if (auth == null) { ShowError(_loc["Auth_Error"]); return; }
            await SaveSessionAsync(auth);
            await Shell.Current.GoToAsync("//MainRibbonPage");
        }
        catch { ShowError(_loc["Auth_Error"]); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task GuestAsync()
    {
        var session = new UserSession { IsGuest = true };
        await _db.SaveSessionAsync(session);
        await Shell.Current.GoToAsync("//MainRibbonPage");
    }

    private void ShowError(string msg) { ErrorMessage = msg; HasError = true; }

    private async Task SaveSessionAsync(AuthResponse auth)
    {
        var session = new UserSession
        {
            IsGuest = false,
            UserId = auth.User.Id,
            DisplayName = auth.User.DisplayName,
            AvatarUrl = auth.User.AvatarUrl ?? string.Empty,
            AccessToken = auth.AccessToken,
            RefreshToken = auth.RefreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(55),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(30),
        };
        await _db.SaveSessionAsync(session);
    }
}

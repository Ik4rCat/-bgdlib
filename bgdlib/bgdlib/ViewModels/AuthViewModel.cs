using System.Security.Cryptography;
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
        LocalizationService.Instance.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
                OnPropertyChanged(nameof(SubmitLabel));
        };
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
            if (IsLogin)
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                { ShowError(_loc["Auth_Error"]); return; }
                if (string.IsNullOrEmpty(Preferences.Default.Get("username", string.Empty)))
                { ShowError(_loc["Auth_Error"]); return; }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                { ShowError(_loc["Auth_Error"]); return; }
                Preferences.Default.Set("username", Name);
                Preferences.Default.Set("user_email", Email);
            }
            Preferences.Default.Set("is_registered", true);
            Preferences.Default.Set("is_guest", false);
            await Shell.Current.GoToAsync("//MainRibbonPage");
        }
        catch { ShowError(_loc["Auth_Error"]); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task LoginGoogleAsync()
    {
        var L = LocalizationService.Instance;
        if (string.IsNullOrEmpty(Constants.GoogleClientId))
        {
            ShowError(L["Auth_GoogleNotConfigured"]);
            return;
        }
        try
        {
            IsBusy = true;
            HasError = false;

            var verifier = GenerateCodeVerifier();
            var challenge = GenerateCodeChallenge(verifier);
            var nonce = Guid.NewGuid().ToString("N");

            var authUrl = new Uri(
                "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={Uri.EscapeDataString(Constants.GoogleClientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(Constants.GoogleRedirectUri)}" +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                $"&code_challenge={challenge}" +
                "&code_challenge_method=S256" +
                $"&nonce={nonce}");

            var result = await WebAuthenticator.Default.AuthenticateAsync(authUrl, new Uri(Constants.GoogleRedirectUri));

            if (!result.Properties.TryGetValue("code", out var code) || string.IsNullOrEmpty(code))
            {
                ShowError(L["Auth_Error"]);
                return;
            }

            var auth = await _api.LoginGoogleWithCodeAsync(code, verifier);
            if (auth == null) { ShowError(L["Auth_Error"]); return; }

            await SaveSessionAsync(auth);
            await Shell.Current.GoToAsync("//MainRibbonPage");
        }
        catch (TaskCanceledException) { }
        catch { ShowError(LocalizationService.Instance["Auth_Error"]); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task GuestAsync()
    {
        Preferences.Default.Set("is_registered", true);
        Preferences.Default.Set("is_guest", true);
        await Shell.Current.GoToAsync("//MainRibbonPage");
    }

    private void ShowError(string msg) { ErrorMessage = msg; HasError = true; }

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        var hash = SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

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

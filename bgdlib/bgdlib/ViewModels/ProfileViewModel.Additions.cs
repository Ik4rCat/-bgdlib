using bgdlib.Models;
using bgdlib.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace bgdlib.ViewModels;

public partial class ProfileViewModel
{
    [ObservableProperty] private string       _profileTab     = "posts";
    [ObservableProperty] private int          _followersCount = 0;
    [ObservableProperty] private int          _followingCount = 0;
    [ObservableProperty] private int          _postsCount     = 0;
    [ObservableProperty] private string       _bio            = "";
    [ObservableProperty] private ImageSource? _avatarSource;
    [ObservableProperty] private string       _joinDate       = string.Empty;
    [ObservableProperty] private int          _savedCount     = 0;

    public bool HasAvatar => AvatarSource is not null;
    partial void OnAvatarSourceChanged(ImageSource? value) => OnPropertyChanged(nameof(HasAvatar));

    public ObservableCollection<ProfileGridItem> PostsGrid { get; } = [];

    [RelayCommand]
    private void GoToPosts()
    {
        ProfileTab = "posts";
        RebuildGrid();
    }

    [RelayCommand]
    private void GoToSaved()
    {
        ProfileTab = "saved";
        RebuildGrid();
    }

    [RelayCommand]
    private void ShareProfile()
    {
        // TODO: share URL when backend has public profile pages
    }

    [RelayCommand]
    private async Task OpenPostAsync(ProfileGridItem item)
    {
        await Shell.Current.GoToAsync($"PostDetailPage?id={item.PostId}");
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "Редактировать профиль", "Введите новое имя:",
            "Сохранить", "Отмена",
            DisplayName, maxLength: 50);
        if (name == null) return;
        name = name.Trim();
        if (string.IsNullOrEmpty(name)) return;
        Preferences.Default.Set("username", name);
        DisplayName = name;
    }

    [RelayCommand]
    private async Task EditEmailAsync()
    {
        var email = await Shell.Current.DisplayPromptAsync(
            "Редактировать профиль", "Введите новый email:",
            "Сохранить", "Отмена",
            Email, maxLength: 100, keyboard: Keyboard.Email);
        if (email == null) return;
        email = email.Trim();
        if (string.IsNullOrEmpty(email)) return;
        Preferences.Default.Set("user_email", email);
        Email = email;
    }

    [RelayCommand]
    private async Task PickAvatarAsync()
    {
        var result = await MediaPicker.Default.PickPhotoAsync();
        if (result is null) return;
        Preferences.Default.Set("avatar_path", result.FullPath);
        AvatarSource = ImageSource.FromFile(result.FullPath);
    }

    [RelayCommand]
    private async Task OpenMenuAsync()
    {
        var L = LocalizationService.Instance;
        var actions = IsGuest
            ? new[] { L["Auth_Login"] }
            : new[] { L["Profile_ClearCache"], L["Profile_Logout"] };

        var result = await Shell.Current.DisplayActionSheetAsync(
            null, L["Cancel"], null, actions);

        if (result == L["Auth_Login"])
            await Shell.Current.GoToAsync(nameof(bgdlib.Views.AuthPage));
        else if (result == L["Profile_ClearCache"])
            await ClearCacheAsync();
        else if (result == L["Profile_Logout"])
            await LogoutAsync();
    }

    private void RebuildGrid()
    {
        PostsGrid.Clear();
        // Placeholder: populate from API when ready
    }
}

using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class CreatePostViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _body = string.Empty;
    [ObservableProperty] private string _tagsRaw = string.Empty;
    [ObservableProperty] private string _engine = "Unity";
    [ObservableProperty] private bool _isBusy;

    public CreatePostViewModel(ApiService api) => _api = api;

    [RelayCommand]
    private Task GoBack() => Shell.Current.GoToAsync("..");

    [RelayCommand]
    private void SetEngine(string engine) => Engine = engine;

    [RelayCommand]
    public async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body))
        {
            await Shell.Current.DisplayAlert("Ошибка", "Заполните заголовок и текст поста.", "OK");
            return;
        }
        IsBusy = true;
        try
        {
            var tags = TagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            await SaveLocallyAsync(Title, Body, tags, Engine);

            bool isGuest = Preferences.Default.Get("is_guest",
                string.IsNullOrEmpty(Preferences.Default.Get("username", "")));
            if (!isGuest)
                await _api.CreatePostAsync(Title, Body, tags, Engine);

            await Shell.Current.DisplayAlert("Готово", "Пост сохранён!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
        }
        finally { IsBusy = false; }
    }

    private static async Task SaveLocallyAsync(string title, string body, string[] tags, string engine)
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "local_posts.json");
        List<LocalPost> posts = [];
        if (File.Exists(path))
        {
            var json = await File.ReadAllTextAsync(path);
            posts = JsonSerializer.Deserialize<List<LocalPost>>(json) ?? [];
        }
        posts.Add(new LocalPost(Guid.NewGuid().ToString(), title, body, tags, engine, DateTime.UtcNow));
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(posts));
    }

    private record LocalPost(string Id, string Title, string Body, string[] Tags, string Engine, DateTime CreatedAt);
}

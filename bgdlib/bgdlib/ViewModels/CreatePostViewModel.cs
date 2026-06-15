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
    public async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body)) return;
        IsBusy = true;
        try
        {
            var tags = TagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var post = await _api.CreatePostAsync(Title, Body, tags, Engine);
            if (post != null)
                await Shell.Current.GoToAsync("..");
        }
        finally { IsBusy = false; }
    }
}

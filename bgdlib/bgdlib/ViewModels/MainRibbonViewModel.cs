using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;
using bgdlib.Views;

namespace bgdlib.ViewModels;

public partial class MainRibbonViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly DatabaseService _db;
    private int _page = 1;
    private int _total = 0;

    [ObservableProperty] private string _selectedEngine = "ALL";
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private bool _isGuest = true;

    public ObservableCollection<CommunityPost> Posts { get; } = [];

    public MainRibbonViewModel(ApiService api, DatabaseService db)
    {
        _api = api;
        _db = db;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        _page = 1;
        Posts.Clear();
        IsGuest = Preferences.Default.Get("is_guest",
            string.IsNullOrEmpty(Preferences.Default.Get("username", "")));
        try
        {
            var page = await _api.GetPostsAsync(1, SelectedEngine == "ALL" ? null : SelectedEngine);
            if (page != null)
            {
                _total = page.Total;
                foreach (var p in page.Items) Posts.Add(p);
            }
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    public async Task SetEngine(string engine)
    {
        if (SelectedEngine == engine) return;
        SelectedEngine = engine;
        await LoadAsync();
    }

    [RelayCommand]
    public async Task LoadMore()
    {
        if (IsLoading || Posts.Count >= _total) return;
        IsLoading = true;
        _page++;
        try
        {
            var page = await _api.GetPostsAsync(_page, SelectedEngine == "ALL" ? null : SelectedEngine);
            if (page != null)
                foreach (var p in page.Items) Posts.Add(p);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task OpenPost(CommunityPost post)
        => await Shell.Current.GoToAsync(nameof(PostDetailPage),
            new Dictionary<string, object> { ["Post"] = post });

    [RelayCommand]
    public async Task CreatePost()
        => await Shell.Current.GoToAsync(nameof(CreatePostPage));

    [RelayCommand]
    public async Task Vote(int postId)
        => await _api.VotePostAsync(postId, 1);
}

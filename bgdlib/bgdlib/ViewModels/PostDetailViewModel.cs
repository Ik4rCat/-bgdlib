using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class PostDetailViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly DatabaseService _db;

    [ObservableProperty] private CommunityPost? _post;
    [ObservableProperty] private string _newComment = string.Empty;
    [ObservableProperty] private bool _canComment;

    public ObservableCollection<PostComment> Comments { get; } = [];

    public PostDetailViewModel(ApiService api, DatabaseService db)
    {
        _api = api;
        _db = db;
    }

    public void SetPost(CommunityPost post)
    {
        Post = post;
        Comments.Clear();
    }

    public async Task LoadCommentsAsync()
    {
        var session = await _db.GetSessionAsync();
        CanComment = !session.IsGuest;
        if (Post == null) return;
        var comments = await _api.GetCommentsAsync(Post.Id);
        if (comments == null) return;
        Comments.Clear();
        foreach (var c in comments) Comments.Add(c);
    }

    [RelayCommand]
    public async Task VoteUp()
    {
        if (Post == null) return;
        await _api.VotePostAsync(Post.Id, 1);
    }

    [RelayCommand]
    public async Task VoteDown()
    {
        if (Post == null) return;
        await _api.VotePostAsync(Post.Id, -1);
    }

    [RelayCommand]
    public async Task SendComment()
    {
        if (Post == null || string.IsNullOrWhiteSpace(NewComment)) return;
        var comment = await _api.CreateCommentAsync(Post.Id, NewComment);
        if (comment != null)
        {
            Comments.Add(comment);
            NewComment = string.Empty;
        }
    }
}

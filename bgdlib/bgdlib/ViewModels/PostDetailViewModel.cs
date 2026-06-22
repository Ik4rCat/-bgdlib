using bgdlib.Models;
using bgdlib.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace bgdlib.ViewModels;

// ── Internal tree node (built from flat API response) ────────────────────────
public class CommentNode
{
    public int    Id       { get; set; }
    public string UserName { get; set; } = "";
    public string Body     { get; set; } = "";
    public string Time     { get; set; } = "";
    public int    Votes    { get; set; }
    public bool   IsOp     { get; set; }
    public List<CommentNode> Replies { get; set; } = [];
}

public partial class PostDetailViewModel : ObservableObject
{
    private readonly ApiService     _api;
    private readonly DatabaseService _db;
    private int? _replyTargetId;

    [ObservableProperty] private CommunityPost? _post;
    [ObservableProperty] private bool   _isLoading;
    [ObservableProperty] private bool   _canComment;
    [ObservableProperty] private string _newComment     = "";
    [ObservableProperty] private bool   _isReplying;
    [ObservableProperty] private string _replyingToLabel = "";
    [ObservableProperty] private string _commentSort    = "best";

    // Original flat list (old pages may still reference this)
    public ObservableCollection<PostComment> Comments     { get; } = [];
    // New threaded flat list for Reddit-style UI
    public ObservableCollection<FlatComment> FlatComments { get; } = [];

    public PostDetailViewModel(ApiService api, DatabaseService db)
    {
        _api = api;
        _db  = db;
    }

    public void SetPost(CommunityPost post)
    {
        Post = post;
        var session = _db.GetSessionAsync().GetAwaiter().GetResult();
        CanComment = session is { IsGuest: false };
    }

    public async Task LoadCommentsAsync()
    {
        if (Post == null) return;
        IsLoading = true;
        try
        {
            var comments = await _api.GetCommentsAsync(Post.Id, CommentSort);
            if (comments == null) return;
            Comments.Clear();
            FlatComments.Clear();
            foreach (var c in comments)
            {
                Comments.Add(c);
                FlatComments.Add(new FlatComment
                {
                    Id        = c.Id,
                    UserName  = c.Author?.DisplayName ?? "?",
                    Body      = c.Body ?? "",
                    TimeLabel = c.CreatedAt.ToString("dd MMM"),
                    BaseVotes = 0,
                    Depth     = 0,
                    RailColor = Microsoft.Maui.Graphics.Colors.Transparent,
                });
            }
        }
        catch { /* silently ignore network errors */ }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SubmitCommentAsync()
    {
        if (Post == null || string.IsNullOrWhiteSpace(NewComment)) return;
        var comment = await _api.CreateCommentAsync(Post.Id, NewComment);
        if (comment != null)
        {
            Comments.Add(comment);
            FlatComments.Add(new FlatComment
            {
                Id        = comment.Id,
                UserName  = comment.Author?.DisplayName ?? "?",
                Body      = comment.Body ?? "",
                TimeLabel = "сейчас",
                BaseVotes = 0,
                Depth     = _replyTargetId.HasValue ? 1 : 0,
            });
        }
        NewComment      = "";
        IsReplying      = false;
        _replyTargetId  = null;
        ReplyingToLabel = "";
    }

    [RelayCommand]
    private void SetReply(FlatComment comment)
    {
        _replyTargetId  = comment.Id;
        IsReplying      = true;
        ReplyingToLabel = $"Ответ → {comment.UserName}";
    }

    [RelayCommand]
    private void CancelReply()
    {
        _replyTargetId  = null;
        IsReplying      = false;
        ReplyingToLabel = "";
    }

    [RelayCommand]
    private async Task SetSortAsync(string sort)
    {
        CommentSort = sort;
        await LoadCommentsAsync();
    }

    [RelayCommand]
    private void ToggleCollapse(FlatComment comment)
    {
        comment.Collapsed = !comment.Collapsed;
    }
}

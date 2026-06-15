namespace bgdlib.Models;

public class ApiUserDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ApiUserDto User { get; set; } = new();
}

public class CommunityPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string? Engine { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public ApiUserDto Author { get; set; } = new();
    public int? MyVote { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CommentCount { get; set; }
}

public class PostComment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Body { get; set; } = string.Empty;
    public ApiUserDto Author { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class PostsPage
{
    public List<CommunityPost> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
}

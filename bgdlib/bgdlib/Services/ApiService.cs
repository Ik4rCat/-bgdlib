using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using bgdlib.Models;

namespace bgdlib.Services;

public class ApiService
{
    private const string BaseUrl = Constants.ApiBaseUrl;
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _http;
    private readonly DatabaseService _db;

    public ApiService(DatabaseService db)
    {
        _db = db;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(15) };
    }

    private async Task AuthorizeAsync()
    {
        var session = await _db.GetSessionAsync();
        if (session.IsGuest) return;
        if (DateTime.UtcNow >= session.AccessTokenExpiresAt.AddMinutes(-5))
            await RefreshAsync(session);
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session.AccessToken);
    }

    private async Task RefreshAsync(UserSession session)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/auth/refresh", new { refreshToken = session.RefreshToken });
            if (!resp.IsSuccessStatusCode) { await _db.ClearSessionAsync(); return; }
            var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>(_json);
            if (auth == null) return;
            session.AccessToken = auth.AccessToken;
            session.RefreshToken = auth.RefreshToken;
            session.AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(55);
            await _db.SaveSessionAsync(session);
        }
        catch { }
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string name, string password)
    {
        var resp = await _http.PostAsJsonAsync("/api/auth/register", new { email, displayName = name, password });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthResponse>(_json);
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("/api/auth/login", new { email, password });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthResponse>(_json);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        try { await _http.PostAsJsonAsync("/api/auth/logout", new { refreshToken }); } catch { }
    }

    public async Task<PostsPage?> GetPostsAsync(int page = 1, string? engine = null, string? tag = null)
    {
        await AuthorizeAsync();
        var query = $"/api/posts?page={page}&size=20";
        if (engine != null && engine != "ALL") query += $"&engine={Uri.EscapeDataString(engine)}";
        if (tag != null) query += $"&tag={Uri.EscapeDataString(tag)}";
        try
        {
            return await _http.GetFromJsonAsync<PostsPage>(query, _json);
        }
        catch { return null; }
    }

    public async Task<CommunityPost?> GetPostAsync(int id)
    {
        await AuthorizeAsync();
        try { return await _http.GetFromJsonAsync<CommunityPost>($"/api/posts/{id}", _json); }
        catch { return null; }
    }

    public async Task<CommunityPost?> CreatePostAsync(string title, string body, string[] tags, string? engine)
    {
        await AuthorizeAsync();
        var resp = await _http.PostAsJsonAsync("/api/posts", new { title, body, tags, engine });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<CommunityPost>(_json);
    }

    public async Task<bool> VotePostAsync(int postId, int value)
    {
        await AuthorizeAsync();
        var resp = await _http.PostAsJsonAsync($"/api/posts/{postId}/vote", new { value });
        return resp.IsSuccessStatusCode;
    }

    public async Task<List<PostComment>?> GetCommentsAsync(int postId)
    {
        await AuthorizeAsync();
        try { return await _http.GetFromJsonAsync<List<PostComment>>($"/api/posts/{postId}/comments", _json); }
        catch { return null; }
    }

    public async Task<PostComment?> CreateCommentAsync(int postId, string body)
    {
        await AuthorizeAsync();
        var resp = await _http.PostAsJsonAsync($"/api/posts/{postId}/comments", new { body });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<PostComment>(_json);
    }
}

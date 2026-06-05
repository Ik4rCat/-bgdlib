using SQLite;
using bgdlib.Models;

namespace bgdlib.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private async Task<SQLiteAsyncConnection> GetDb()
    {
        if (_db is not null) return _db;

        await _initLock.WaitAsync();
        try
        {
            if (_db is not null) return _db;
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bgdlib.db3");
            _db = new SQLiteAsyncConnection(dbPath);
            await _db.CreateTableAsync<FeedItem>();
            await _db.CreateTableAsync<FavoriteItem>();
            await _db.CreateTableAsync<Note>();
            await _db.CreateTableAsync<UserSession>();
        }
        finally
        {
            _initLock.Release();
        }
        return _db;
    }

    // ── FeedItems ─────────────────────────────────────────────────────────────

    public async Task<List<FeedItem>> GetFeedItemsAsync()
    {
        var db = await GetDb();
        return await db.Table<FeedItem>().OrderByDescending(x => x.PublishedAt).ToListAsync();
    }

    public async Task SaveFeedItemsAsync(IEnumerable<FeedItem> items)
    {
        var db = await GetDb();
        foreach (var item in items)
            await db.InsertOrReplaceAsync(item);
    }

    public async Task ClearFeedCacheAsync()
    {
        var db = await GetDb();
        await db.DeleteAllAsync<FeedItem>();
    }

    public async Task<long> GetCacheSizeBytesAsync()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bgdlib.db3");
        return await Task.Run(() => new FileInfo(dbPath).Exists ? new FileInfo(dbPath).Length : 0);
    }

    // ── Favorites ─────────────────────────────────────────────────────────────

    public async Task<List<FavoriteItem>> GetFavoritesAsync()
    {
        var db = await GetDb();
        return await db.Table<FavoriteItem>().OrderByDescending(x => x.SavedAt).ToListAsync();
    }

    public async Task AddFavoriteAsync(FavoriteItem item)
    {
        var db = await GetDb();
        await db.InsertAsync(item);
    }

    public async Task RemoveFavoriteAsync(string url)
    {
        var db = await GetDb();
        await db.ExecuteAsync("DELETE FROM Favorites WHERE Url = ?", url);
    }

    public async Task<bool> IsFavoriteAsync(string url)
    {
        var db = await GetDb();
        return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Favorites WHERE Url = ?", url) > 0;
    }

    // ── Notes ─────────────────────────────────────────────────────────────────

    public async Task<List<Note>> GetNotesAsync()
    {
        var db = await GetDb();
        return await db.Table<Note>().OrderByDescending(x => x.UpdatedAt).ToListAsync();
    }

    public async Task<int> SaveNoteAsync(Note note)
    {
        var db = await GetDb();
        note.UpdatedAt = DateTime.UtcNow;
        if (note.Id == 0)
            return await db.InsertAsync(note);
        return await db.UpdateAsync(note);
    }

    public async Task DeleteNoteAsync(Note note)
    {
        var db = await GetDb();
        if (File.Exists(note.FilePath))
            File.Delete(note.FilePath);
        await db.DeleteAsync(note);
    }

    // ── UserSession ───────────────────────────────────────────────────────────

    public async Task<UserSession> GetSessionAsync()
    {
        var db = await GetDb();
        return await db.FindAsync<UserSession>(1) ?? new UserSession();
    }

    public async Task SaveSessionAsync(UserSession session)
    {
        var db = await GetDb();
        await db.InsertOrReplaceAsync(session);
    }
}

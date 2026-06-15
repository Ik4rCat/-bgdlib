using SQLite;

namespace bgdlib.Models;

[Table("FeedItems")]
public class FeedItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;    // "Unity Blog", "r/gamedev" и т.д.
    public string Category { get; set; } = string.Empty;  // "tutorial", "news", "job" и т.д.
    public string Engine { get; set; } = string.Empty;    // "Unity", "Godot", "Unreal", "Other"
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    [SQLite.Ignore]
    public bool IsFavorite { get; set; }
}

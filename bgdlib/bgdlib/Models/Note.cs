using SQLite;

namespace bgdlib.Models;

[Table("Notes")]
public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;  // путь к .md файлу
    public string Tags { get; set; } = string.Empty;      // через запятую: "unity,gamedev"
    public string LinkedArticleUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

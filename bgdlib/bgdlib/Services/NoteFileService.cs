namespace bgdlib.Services;

public class NoteFileService
{
    private readonly string _notesDir;

    public NoteFileService()
    {
        _notesDir = Path.Combine(FileSystem.AppDataDirectory, "notes");
        Directory.CreateDirectory(_notesDir);
    }

    public async Task<string> ReadAsync(string filePath)
    {
        if (!File.Exists(filePath)) return string.Empty;
        return await File.ReadAllTextAsync(filePath);
    }

    public async Task<string> WriteAsync(string title, string content, string? existingPath = null)
    {
        var path = existingPath ?? Path.Combine(_notesDir, $"{SanitizeTitle(title)}_{DateTime.Now:yyyyMMddHHmmss}.md");
        await File.WriteAllTextAsync(path, content);
        return path;
    }

    public async Task ExportAsync(string filePath)
    {
        // FileSaver из CommunityToolkit.Maui — пока заглушка
        // TODO: подключить CommunityToolkit.Maui и использовать FileSaver.SaveAsync
        var fileName = Path.GetFileName(filePath);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Экспорт заметки",
            File = new ShareFile(filePath)
        });
    }

    private static string SanitizeTitle(string title)
        => string.Join("_", title.Split(Path.GetInvalidFileNameChars())).Trim().Replace(" ", "_");
}

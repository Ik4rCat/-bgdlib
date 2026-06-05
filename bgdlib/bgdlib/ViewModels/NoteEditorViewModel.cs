using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class NoteEditorViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly NoteFileService _fileService;

    [ObservableProperty] private string _title = "Новая заметка";
    [ObservableProperty] private string _tags = string.Empty;
    [ObservableProperty] private string _markdownContent = string.Empty;
    [ObservableProperty] private bool _isSaving;

    public Note? CurrentNote { get; set; }

    // Событие для передачи контента в WebView
    public event Action<string>? ContentLoaded;

    public NoteEditorViewModel(DatabaseService db, NoteFileService fileService)
    {
        _db = db;
        _fileService = fileService;
    }

    public async Task InitAsync(Note? note = null)
    {
        CurrentNote = note;
        if (note is not null)
        {
            Title = note.Title;
            Tags = note.Tags;
            MarkdownContent = await _fileService.ReadAsync(note.FilePath);
        }
        else
        {
            MarkdownContent = "# Новая заметка\n\n";
        }
        ContentLoaded?.Invoke(MarkdownContent);
    }

    [RelayCommand]
    public async Task SaveAsync(string content)
    {
        IsSaving = true;
        MarkdownContent = content;

        var path = await _fileService.WriteAsync(Title, content, CurrentNote?.FilePath);

        var note = CurrentNote ?? new Note { CreatedAt = DateTime.UtcNow };
        note.Title = Title;
        note.Tags = Tags;
        note.FilePath = path;

        await _db.SaveNoteAsync(note);
        CurrentNote = note;
        IsSaving = false;
    }

    [RelayCommand]
    public async Task ExportAsync()
    {
        if (CurrentNote?.FilePath is not null)
            await _fileService.ExportAsync(CurrentNote.FilePath);
    }
}

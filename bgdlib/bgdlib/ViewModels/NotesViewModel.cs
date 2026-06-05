using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private bool _isEmpty;

    public ObservableCollection<Note> Notes { get; } = [];
    private List<Note> _allNotes = [];

    public NotesViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allNotes = await _db.GetNotesAsync();
        ApplySearch();
    }

    partial void OnSearchQueryChanged(string value) => ApplySearch();

    private void ApplySearch()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? _allNotes
            : _allNotes.Where(n =>
                n.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                n.Tags.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        Notes.Clear();
        foreach (var n in filtered) Notes.Add(n);
        IsEmpty = Notes.Count == 0;
    }

    [RelayCommand]
    public async Task DeleteAsync(Note note)
    {
        await _db.DeleteNoteAsync(note);
        _allNotes.Remove(note);
        ApplySearch();
    }
}

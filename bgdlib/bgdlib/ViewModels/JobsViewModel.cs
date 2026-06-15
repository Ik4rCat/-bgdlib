using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;
using bgdlib.Services;

namespace bgdlib.ViewModels;

public partial class JobsViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private List<FeedItem> _allJobs = [];

    [ObservableProperty] private string _selectedLevel = "Все";
    [ObservableProperty] private string _selectedEngine = "Все";
    [ObservableProperty] private bool _remoteOnly = false;
    [ObservableProperty] private bool _isEmpty = false;

    public List<string> Levels { get; } = ["Все", "Junior", "Middle", "Senior"];
    public List<string> Engines { get; } = ["Все", "Unity", "Godot", "Unreal", "Other"];
    public ObservableCollection<FeedItem> Jobs { get; } = [];

    public JobsViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allJobs = (await _db.GetFeedItemsAsync())
            .Where(x => x.Category == "job")
            .ToList();
        ApplyFilters();
    }

    [RelayCommand]
    public void SetLevel(string level) { SelectedLevel = level; ApplyFilters(); }

    [RelayCommand]
    public void SetEngine(string engine) { SelectedEngine = engine; ApplyFilters(); }

    [RelayCommand]
    public void ToggleRemote() { RemoteOnly = !RemoteOnly; ApplyFilters(); }

    private void ApplyFilters()
    {
        var filtered = _allJobs.AsEnumerable();
        if (SelectedEngine != "Все")
            filtered = filtered.Where(x => x.Engine == SelectedEngine);
        if (SelectedLevel != "Все")
            filtered = filtered.Where(x =>
                x.Title.Contains(SelectedLevel, StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(SelectedLevel, StringComparison.OrdinalIgnoreCase));
        if (RemoteOnly)
            filtered = filtered.Where(x =>
                x.Title.Contains("remote", StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains("remote", StringComparison.OrdinalIgnoreCase) ||
                x.Title.Contains("удалённо", StringComparison.OrdinalIgnoreCase));

        Jobs.Clear();
        foreach (var job in filtered) Jobs.Add(job);
        IsEmpty = Jobs.Count == 0;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using bgdlib.Models;

namespace bgdlib.ViewModels;

public partial class DocsViewModel : ObservableObject
{
    [ObservableProperty] private string _selectedEngine = "Все";

    public List<string> Engines { get; } = ["Все", "Unity", "Godot", "Unreal", "Other"];

    private static readonly List<DocLink> AllDocs =
    [
        new() { Title = "Unity Manual", Url = "https://docs.unity3d.com/Manual/",
                Engine = "Unity", Author = "Unity Technologies",
                Description = "Полное руководство по движку Unity",
                Category = "manual", AccentColor = "#a6e3a1" },
        new() { Title = "Unity Scripting API", Url = "https://docs.unity3d.com/ScriptReference/",
                Engine = "Unity", Author = "Unity Technologies",
                Description = "Справочник по C# API Unity",
                Category = "api", AccentColor = "#a6e3a1" },
        new() { Title = "Unity Learn", Url = "https://learn.unity.com/",
                Engine = "Unity", Author = "Unity Technologies",
                Description = "Официальные курсы и туториалы",
                Category = "tutorials", AccentColor = "#a6e3a1" },
        new() { Title = "Catlike Coding (Unity)", Url = "https://catlikecoding.com/unity/tutorials/",
                Engine = "Unity", Author = "Jasper Flick",
                Description = "Продвинутые туториалы по шейдерам и рендерингу",
                Category = "tutorials", AccentColor = "#a6e3a1" },
        new() { Title = "Godot Documentation", Url = "https://docs.godotengine.org/",
                Engine = "Godot", Author = "Godot Engine",
                Description = "Документация по GDScript и C# API",
                Category = "manual", AccentColor = "#89b4fa" },
        new() { Title = "Godot Best Practices", Url = "https://docs.godotengine.org/en/stable/tutorials/best_practices/",
                Engine = "Godot", Author = "Godot Engine",
                Description = "Лучшие практики разработки на Godot",
                Category = "patterns", AccentColor = "#89b4fa" },
        new() { Title = "Godot Recipes", Url = "https://kidscancode.org/godot_recipes/",
                Engine = "Godot", Author = "kidscancode.org",
                Description = "Практические рецепты для разработки на Godot",
                Category = "patterns", AccentColor = "#89b4fa" },
        new() { Title = "Unreal Engine Docs", Url = "https://dev.epicgames.com/documentation/",
                Engine = "Unreal", Author = "Epic Games",
                Description = "Документация Unreal Engine 5",
                Category = "manual", AccentColor = "#f38ba8" },
        new() { Title = "Blueprint Visual Scripting", Url = "https://dev.epicgames.com/documentation/en-us/unreal-engine/blueprints-visual-scripting-in-unreal-engine",
                Engine = "Unreal", Author = "Epic Games",
                Description = "Визуальное программирование в Unreal",
                Category = "api", AccentColor = "#f38ba8" },
        new() { Title = "GDC Vault (Free)", Url = "https://gdcvault.com/free",
                Engine = "Other", Author = "Game Developers Conference",
                Description = "Бесплатные доклады с GDC",
                Category = "talks", AccentColor = "#f9e2af" },
        new() { Title = "Game Programming Patterns", Url = "https://gameprogrammingpatterns.com/",
                Engine = "Other", Author = "Robert Nystrom",
                Description = "Паттерны программирования для игр (онлайн-книга бесплатно)",
                Category = "patterns", AccentColor = "#f9e2af" },
        new() { Title = "80.lv Articles", Url = "https://80.lv/articles/",
                Engine = "Other", Author = "80.lv",
                Description = "Статьи по 3D арту, шейдерам и геймдеву",
                Category = "manual", AccentColor = "#cba6f7" },
    ];

    public ObservableCollection<DocLink> Docs { get; } = [];

    [RelayCommand]
    public void SetEngineDocs(string engine) => SelectedEngine = engine;

    [RelayCommand]
    public async Task OpenDoc(DocLink doc) =>
        await Launcher.OpenAsync(new Uri(doc.Url));

    partial void OnSelectedEngineChanged(string v) => ApplyFilter();

    public void Init() => ApplyFilter();

    private void ApplyFilter()
    {
        Docs.Clear();
        var filtered = SelectedEngine == "Все"
            ? AllDocs
            : AllDocs.Where(x => x.Engine == SelectedEngine);
        foreach (var doc in filtered) Docs.Add(doc);
    }
}

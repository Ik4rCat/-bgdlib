using System.ComponentModel;
using System.Globalization;

namespace bgdlib.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private static LocalizationService? _instance;
    public static LocalizationService Instance => _instance ??= new LocalizationService();

    private string _lang = "ru";

    private static readonly Dictionary<string, Dictionary<string, string>> _t = new()
    {
        ["en"] = new()
        {
            // Tabs
            ["Tab_Feed"]    = "Feed",
            ["Tab_Search"]  = "Search",
            ["Tab_Saved"]   = "Saved",
            ["Tab_Jobs"]    = "Jobs",
            ["Tab_Docs"]    = "Docs",
            ["Tab_Notes"]   = "Notes",
            ["Tab_Profile"] = "Profile",
            // Filters
            ["Filter_All"]        = "All",
            ["Filter_AllEngines"] = "All engines",
            ["Filter_AllLevels"]  = "All levels",
            // Engine / Category names
            ["Cat_Unity"]       = "Unity",
            ["Cat_Godot"]       = "Godot",
            ["Cat_Unreal"]      = "Unreal",
            ["Cat_Other"]       = "Other",
            ["Cat_Tutorials"]   = "Tutorials",
            ["Cat_Jobs"]        = "Jobs",
            ["Cat_Tools"]       = "Tools",
            ["Cat_Postmortems"] = "Postmortems",
            ["Cat_Docs"]        = "Documentation",
            // FeedPage
            ["Feed_Empty"]        = "No articles. Pull down to refresh.",
            ["Feed_NoNetwork"]    = "No network",
            ["Feed_NoNetworkCache"] = "No network — showing cache",
            ["Feed_Updated"]      = "Updated: {0} articles",
            ["Feed_Loading"]      = "Loading feed...",
            // FavoritesPage
            ["Favs_Empty"] = "No saved articles",
            // SearchPage
            ["Search_Placeholder"] = "Unity, Godot, game design ...",
            ["Search_Categories"]  = "CATEGORIES",
            ["Search_Tags"]        = "POPULAR TAGS",
            ["Search_NoResults"]   = "Nothing found",
            // JobsPage
            ["Jobs_Title"]   = "Jobs",
            ["Jobs_Remote"]  = "Remote",
            ["Jobs_Empty"]   = "No jobs found\nUpdate feed on main tab",
            // DocsPage
            ["Docs_Title"] = "Documentation",
            // ProfilePage
            ["Profile_Guest"]        = "Guest",
            ["Profile_GuestNote"]    = "Guest mode — data stored locally",
            ["Profile_LoginBtn"]     = "Sign in with Google",
            ["Profile_Cache"]        = "Feed cache",
            ["Profile_ClearCache"]   = "Clear",
            ["Profile_CacheCleared"] = "Cache cleared",
            ["Profile_CacheClearedMsg"] = "Feed articles removed from cache",
            ["Profile_Lang"]    = "Language",
            ["Profile_LangRU"]  = "Русский",
            ["Profile_LangEN"]  = "English",
            ["Swipe_Delete"] = "Delete",
            ["OK"] = "OK",
            // Popular tags
            ["Tags"] = "C#|Unity|Godot|Shaders|AI / NPC|Level Design|Blueprints|Patterns|Pixel Art|GDScript",
        },
        ["ru"] = new()
        {
            // Tabs
            ["Tab_Feed"]    = "Лента",
            ["Tab_Search"]  = "Поиск",
            ["Tab_Saved"]   = "Сохранено",
            ["Tab_Jobs"]    = "Вакансии",
            ["Tab_Docs"]    = "Доки",
            ["Tab_Notes"]   = "Заметки",
            ["Tab_Profile"] = "Профиль",
            // Filters
            ["Filter_All"]        = "Все",
            ["Filter_AllEngines"] = "Все движки",
            ["Filter_AllLevels"]  = "Все уровни",
            // Engine / Category names
            ["Cat_Unity"]       = "Unity",
            ["Cat_Godot"]       = "Godot",
            ["Cat_Unreal"]      = "Unreal",
            ["Cat_Other"]       = "Other",
            ["Cat_Tutorials"]   = "Туториалы",
            ["Cat_Jobs"]        = "Вакансии",
            ["Cat_Tools"]       = "Инструменты",
            ["Cat_Postmortems"] = "Постмортемы",
            ["Cat_Docs"]        = "Документации",
            // FeedPage
            ["Feed_Empty"]        = "Нет статей. Потяните вниз для загрузки.",
            ["Feed_NoNetwork"]    = "Нет сети",
            ["Feed_NoNetworkCache"] = "Нет сети — показан кэш",
            ["Feed_Updated"]      = "Обновлено: {0} статей",
            ["Feed_Loading"]      = "Загрузка ленты...",
            // FavoritesPage
            ["Favs_Empty"] = "Нет сохранённых статей",
            // SearchPage
            ["Search_Placeholder"] = "Unity, Godot, геймдизайн ...",
            ["Search_Categories"]  = "КАТЕГОРИИ",
            ["Search_Tags"]        = "ПОПУЛЯРНЫЕ ТЕГИ",
            ["Search_NoResults"]   = "Ничего не найдено",
            // JobsPage
            ["Jobs_Title"]   = "Вакансии",
            ["Jobs_Remote"]  = "Remote",
            ["Jobs_Empty"]   = "Вакансий не найдено\nОбновите ленту на главной",
            // DocsPage
            ["Docs_Title"] = "Документации",
            // ProfilePage
            ["Profile_Guest"]        = "Гость",
            ["Profile_GuestNote"]    = "Режим гостя — данные хранятся локально",
            ["Profile_LoginBtn"]     = "Войти через Google",
            ["Profile_Cache"]        = "Кэш ленты",
            ["Profile_ClearCache"]   = "Очистить",
            ["Profile_CacheCleared"] = "Кэш очищен",
            ["Profile_CacheClearedMsg"] = "Статьи ленты удалены из кэша",
            ["Profile_Lang"]    = "Язык",
            ["Profile_LangRU"]  = "Русский",
            ["Profile_LangEN"]  = "English",
            ["Swipe_Delete"] = "Удалить",
            ["OK"] = "OK",
            // Popular tags
            ["Tags"] = "C#|Unity|Godot|Шейдеры|AI / NPC|Level Design|Blueprints|Паттерны|Pixel Art|GDScript",
        }
    };

    public LocalizationService()
    {
        _instance ??= this;
        var saved = Preferences.Default.Get("lang", "");
        if (string.IsNullOrEmpty(saved))
        {
            saved = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "ru" : "en";
        }
        _lang = saved;
    }

    public string CurrentLanguage => _lang;

    public string this[string key]
    {
        get
        {
            if (_t.TryGetValue(_lang, out var dict) && dict.TryGetValue(key, out var val))
                return val;
            if (_t.TryGetValue("en", out var en) && en.TryGetValue(key, out var enVal))
                return enVal;
            return key;
        }
    }

    public string Format(string key, params object[] args) =>
        string.Format(this[key], args);

    public List<string> GetTags() =>
        this["Tags"].Split('|').ToList();

    public void SetLanguage(string lang)
    {
        if (_lang == lang) return;
        _lang = lang;
        Preferences.Default.Set("lang", lang);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

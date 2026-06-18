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
            ["Tab_Feed"]    = "News",
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
            // Main Ribbon
            ["Tab_Main"]       = "Main",
            ["Ribbon_Empty"]   = "No posts yet. Be the first!",
            ["Ribbon_CreatePost"] = "New Post",
            // Auth
            ["Auth_Login"]    = "Sign In",
            ["Auth_Register"] = "Register",
            ["Auth_Email"]    = "Email",
            ["Auth_Password"] = "Password",
            ["Auth_Name"]     = "Display Name",
            ["Auth_GuestMode"] = "Continue as guest",
            ["Auth_Error"]    = "Error. Check your details.",
            ["Auth_GoogleNotConfigured"] = "Google Sign-In is not configured",
            ["Auth_LoginGoogle"] = "Continue with Google",
            // Post
            ["Post_Title"]      = "Title",
            ["Post_Body"]       = "Content (Markdown)",
            ["Post_Tags"]       = "Tags (comma separated)",
            ["Post_Engine"]     = "Engine",
            ["Post_Submit"]     = "Publish",
            ["Post_Comments"]   = "COMMENTS",
            ["Post_AddComment"] = "Add a comment...",
            // Profile
            ["Profile_Logout"]          = "Sign Out",
            ["Profile_SectionContent"]  = "CONTENT",
            ["Profile_SectionSettings"] = "SETTINGS",
            ["Profile_Title"]           = "Profile",
            ["Profile_Posts"]           = "Posts",
            ["Profile_Read"]            = "Read",
            ["Profile_Tags"]            = "Tags",
            ["Profile_Favorites"]       = "Favorites",
            ["Profile_FavoritesDesc"]   = "Saved articles",
            ["Profile_DocsDesc"]        = "Engine documentation",
            ["Profile_NotesDesc"]       = "Personal notes",
            // Auth
            ["Auth_Subtitle"]           = "Game dev platform for developers",
            // Feed
            ["Feed_RibbonWord"]         = "Feed",
            // Docs
            ["Docs_Subtitle"]           = "Documentation and resources",
            // Notes
            ["Notes_Title"]             = "Notes",
            ["Notes_Search"]            = "Search notes and tags...",
            ["Notes_Empty"]             = "No notes. Tap + to create.",
            // Article
            ["Article_Title"]           = "Article",
            ["Article_AddFavorite"]     = "Add to Favorites",
            ["Article_AlreadySaved"]    = "Already saved",
            ["Article_AlreadySavedMsg"] = "This article is already saved",
            ["Article_Saved"]           = "Saved",
            ["Article_SavedMsg"]        = "Article added to favorites",
        },
        ["ru"] = new()
        {
            // Tabs
            ["Tab_Feed"]    = "Новости",
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
            // Main Ribbon
            ["Tab_Main"]       = "Лента",
            ["Ribbon_Empty"]   = "Постов пока нет. Будьте первым!",
            ["Ribbon_CreatePost"] = "Новый пост",
            // Auth
            ["Auth_Login"]    = "Войти",
            ["Auth_Register"] = "Регистрация",
            ["Auth_Email"]    = "Email",
            ["Auth_Password"] = "Пароль",
            ["Auth_Name"]     = "Имя",
            ["Auth_GuestMode"] = "Продолжить как гость",
            ["Auth_Error"]    = "Ошибка. Проверьте данные.",
            ["Auth_GoogleNotConfigured"] = "Google Sign-In не настроен",
            ["Auth_LoginGoogle"] = "Войти через Google",
            // Post
            ["Post_Title"]      = "Заголовок",
            ["Post_Body"]       = "Содержание (Markdown)",
            ["Post_Tags"]       = "Теги (через запятую)",
            ["Post_Engine"]     = "Движок",
            ["Post_Submit"]     = "Опубликовать",
            ["Post_Comments"]   = "КОММЕНТАРИИ",
            ["Post_AddComment"] = "Добавить комментарий...",
            // Profile
            ["Profile_Logout"]          = "Выйти",
            ["Profile_SectionContent"]  = "КОНТЕНТ",
            ["Profile_SectionSettings"] = "НАСТРОЙКИ",
            ["Profile_Title"]           = "Профиль",
            ["Profile_Posts"]           = "Постов",
            ["Profile_Read"]            = "Прочитано",
            ["Profile_Tags"]            = "Тэги",
            ["Profile_Favorites"]       = "Избранное",
            ["Profile_FavoritesDesc"]   = "Сохранённые материалы",
            ["Profile_DocsDesc"]        = "Документации движков",
            ["Profile_NotesDesc"]       = "Персональные заметки",
            // Auth
            ["Auth_Subtitle"]           = "Геймдев-платформа для разработчиков",
            // Feed
            ["Feed_RibbonWord"]         = "Лента",
            // Docs
            ["Docs_Subtitle"]           = "Документации и ресурсы",
            // Notes
            ["Notes_Title"]             = "Заметки",
            ["Notes_Search"]            = "Поиск по заметкам и тегам...",
            ["Notes_Empty"]             = "Нет заметок. Нажми + чтобы создать.",
            // Article
            ["Article_Title"]           = "Статья",
            ["Article_AddFavorite"]     = "В избранное",
            ["Article_AlreadySaved"]    = "Уже в избранном",
            ["Article_AlreadySavedMsg"] = "Эта статья уже сохранена",
            ["Article_Saved"]           = "Сохранено",
            ["Article_SavedMsg"]        = "Статья добавлена в избранное",
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

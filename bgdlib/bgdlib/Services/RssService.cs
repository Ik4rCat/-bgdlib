using CodeHollow.FeedReader;
using bgdlib.Models;
using FeedItem = bgdlib.Models.FeedItem;

namespace bgdlib.Services;

public class RssService
{
    public static readonly List<(string Name, string Url, string Engine)> Sources =
    [
        ("Unity Blog",       "https://unity.com/blog/rss.xml",                              "Unity"),
        ("Godot News",       "https://godotengine.org/rss.xml",                              "Godot"),
        ("Unreal Blog",      "https://www.unrealengine.com/en-US/rss",                       "Unreal"),
        ("Gamasutra",        "https://www.gamedeveloper.com/rss.xml",                        "Other"),
        ("r/gamedev",        "https://www.reddit.com/r/gamedev/.rss",                        "Other"),
        ("r/unity",          "https://www.reddit.com/r/Unity3D/.rss",                        "Unity"),
        ("GameDev Jobs",     "https://www.reddit.com/r/gamedevclassifieds/.rss",             "Other"),
        ("itch.io Jobs",     "https://itch.io/jobs.rss",                                     "Other"),
        ("Game From Scratch","https://gamefromscratch.com/feed/",                             "Other"),
        ("80.lv",            "https://80.lv/feed/",                                          "Other"),
        ("Habr Gamedev",     "https://habr.com/ru/rss/hubs/gamedev/articles/",              "Other"),
    ];

    public async Task<List<FeedItem>> FetchAllAsync(IProgress<string>? progress = null)
    {
        var result = new List<FeedItem>();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BGDLib/1.0 (Android; gamedev aggregator)");
        httpClient.Timeout = TimeSpan.FromSeconds(15);

        foreach (var (name, url, engine) in Sources)
        {
            try
            {
                progress?.Report($"Загрузка {name}...");
                var xml = await httpClient.GetStringAsync(url);
                var feed = FeedReader.ReadFromString(xml);
                foreach (var item in feed.Items.Take(20))
                {
                    result.Add(new FeedItem
                    {
                        Title       = item.Title ?? "",
                        Url         = item.Link ?? "",
                        Description = StripHtml(item.Description ?? ""),
                        Source      = name,
                        Engine      = engine,
                        Category    = GuessCategory(item.Title ?? "", item.Description ?? ""),
                        ImageUrl    = ExtractImage(item.Description ?? ""),
                        PublishedAt = item.PublishingDate ?? DateTime.UtcNow,
                    });
                }
            }
            catch
            {
                // пропускаем недоступный источник
            }
        }

        return result.OrderByDescending(x => x.PublishedAt).ToList();
    }

    private static string GuessCategory(string title, string desc)
    {
        var text = (title + " " + desc).ToLowerInvariant();

        if (text.Contains("tutorial") || text.Contains("how to") || text.Contains("guide") ||
            text.Contains("туториал") || text.Contains("урок") || text.Contains("руководство") ||
            text.Contains("как сделать") || text.Contains("научитесь"))
            return "tutorial";

        if (text.Contains("job") || text.Contains("hiring") || text.Contains("vacancy") ||
            text.Contains("вакансия") || text.Contains("ищем") || text.Contains("разработчик нужен") ||
            text.Contains("gamedevclassifieds") || text.Contains("remote"))
            return "job";

        if (text.Contains("tool") || text.Contains("asset") || text.Contains("plugin") ||
            text.Contains("инструмент") || text.Contains("плагин") || text.Contains("пакет"))
            return "tool";

        if (text.Contains("postmortem") || text.Contains("post-mortem") ||
            text.Contains("постмортем") || text.Contains("как мы делали"))
            return "postmortem";

        if (text.Contains("docs") || text.Contains("documentation") || text.Contains("api reference") ||
            text.Contains("документация") || text.Contains("справочник"))
            return "docs";

        return "news";
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html)) return html;
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", "").Trim();
    }

    private static string ExtractImage(string html)
    {
        var match = System.Text.RegularExpressions.Regex.Match(html, @"<img[^>]+src=""([^""]+)""");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}

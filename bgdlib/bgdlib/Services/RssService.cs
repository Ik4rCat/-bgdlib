using CodeHollow.FeedReader;
using bgdlib.Models;
using FeedItem = bgdlib.Models.FeedItem;

namespace bgdlib.Services;

public class RssService
{
    public static readonly List<(string Name, string Url, string Engine)> Sources =
    [
        ("Unity Blog",       "https://unity.com/blog/rss.xml",                         "Unity"),
        ("Godot News",       "https://godotengine.org/rss.xml",                         "Godot"),
        ("Gamasutra",        "https://www.gamedeveloper.com/rss.xml",                   "Other"),
        ("r/gamedev",        "https://www.reddit.com/r/gamedev/.rss",                   "Other"),
        ("r/unity",          "https://www.reddit.com/r/Unity3D/.rss",                   "Unity"),
        ("Game From Scratch","https://gamefromscratch.com/feed/",                        "Other"),
    ];

    public async Task<List<FeedItem>> FetchAllAsync(IProgress<string>? progress = null)
    {
        var result = new List<FeedItem>();

        foreach (var (name, url, engine) in Sources)
        {
            try
            {
                progress?.Report($"Загрузка {name}...");
                var feed = await FeedReader.ReadAsync(url);
                foreach (var item in feed.Items.Take(20))
                {
                    result.Add(new FeedItem
                    {
                        Title = item.Title ?? "",
                        Url = item.Link ?? "",
                        Description = StripHtml(item.Description ?? ""),
                        Source = name,
                        Engine = engine,
                        Category = GuessCategory(item.Title ?? "", item.Description ?? ""),
                        ImageUrl = ExtractImage(item.Description ?? ""),
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
        if (text.Contains("tutorial") || text.Contains("how to") || text.Contains("guide"))
            return "tutorial";
        if (text.Contains("job") || text.Contains("hiring") || text.Contains("вакансия"))
            return "job";
        if (text.Contains("postmortem") || text.Contains("post-mortem"))
            return "postmortem";
        if (text.Contains("tool") || text.Contains("asset") || text.Contains("plugin"))
            return "tool";
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

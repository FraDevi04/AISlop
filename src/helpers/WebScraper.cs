// src/helpers/WebScraper.cs
namespace AISlop;

using AngleSharp;
using AngleSharp.Dom;
using System.Text;
using System.Text.Json;
using System.Web;

public record SearchResult(string Title, string Link, string Snippet);
public static class WebScraper
{
    private static string apiKey = Config.Settings.search_api_key;
    private static string searchEngineId = Config.Settings.search_engine_id;
    static WebScraper()
    {
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    }
    private static readonly HttpClient client = new HttpClient();
    public static async Task<string> Search(string query)
    {
        try
        {
            var results = await PerformSearch(query);
            if (results == null || results.Count == 0)
                return "No search results found.";

            StringBuilder sb = new();
            int count = 1;

            foreach (var result in results)
            {
                sb.AppendLine($"{count++}. {result.Title}");
                sb.AppendLine($"\tLink: {result.Link}");
                sb.AppendLine($"\tSnippet: {result.Snippet}");
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static async Task<List<SearchResult>> PerformSearch(string query)
    {
        string encodedQuery = HttpUtility.UrlEncode(query);

        string searchUrl = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={searchEngineId}&q={encodedQuery}";

        string jsonResponse = await client.GetStringAsync(searchUrl);

        using JsonDocument doc = JsonDocument.Parse(jsonResponse);
        var results = new List<SearchResult>();

        if (doc.RootElement.TryGetProperty("items", out JsonElement items))
        {
            foreach (JsonElement item in items.EnumerateArray())
            {
                string title = item.GetProperty("title").GetString() ?? "";
                string link = item.GetProperty("link").GetString() ?? "";
                string snippet = item.GetProperty("snippet").GetString() ?? "";

                results.Add(new SearchResult(title, link, snippet));
            }
        }

        return results;
    }

    /// <summary>
    /// Downloads a webpage and extracts its meaningful text content.
    /// </summary>
    /// <param name="url">The URL of the webpage to scrape.</param>
    /// <returns>A string containing the cleaned text content of the page.</returns>
    public static async Task<string> ScrapeTextFromUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            return "Invalid url provided.";

        try
        {
            string htmlContent = await client.GetStringAsync(url);

            var context = BrowsingContext.New(Configuration.Default);
            IDocument document = await context.OpenAsync(req => req.Content(htmlContent));

            var elementsToRemove = document.QuerySelectorAll("script, style, nav, header, footer, aside");
            foreach (var element in elementsToRemove)
            {
                element.Remove();
            }
            string rawText = document.Body?.TextContent ?? string.Empty;

            var lines = rawText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    builder.AppendLine(trimmedLine);
                }
            }

            return builder.ToString();
        }
        catch (HttpRequestException e)
        {
            return $"Failed to download content from {url}. Status: {e.StatusCode}";
        }
        catch (Exception)
        {
            return $"An unexpected error occurred while scraping {url}.";
        }
    }
}

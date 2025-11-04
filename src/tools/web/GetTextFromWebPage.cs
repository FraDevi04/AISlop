// src/tools/web/GetTextFromWebPage.cs
using AISlop;

namespace AISlop;

public class GetTextFromWebPage : ITool
{
    public string Name => "gettextfromwebpage";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        string? url = args.GetValueOrDefault("url");
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult("Error: 'url' is required.");
        return _GetTextFromWebPage(url);
    }

    /// <summary>
    /// TODO: doesn't work for online pdf files. c# has to download it into a temp folder
    /// then read it with the PDFReader THEN return the page details
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private async Task<string> _GetTextFromWebPage(string url)
    {
        return await WebScraper.ScrapeTextFromUrlAsync(url);
    }
}

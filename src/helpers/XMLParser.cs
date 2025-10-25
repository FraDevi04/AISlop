using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AISlop;
public static class XMLParser
{
    public static Command ParseCurrent(ParserContext context)
    {
        while (true)
        {
            var startMatch = Regex.Match( // Has to be perfect match
                context.Buffer.Substring(context.SearchPosition),
                @"<tool_call name=""(?<name>.*?)"">"
            );

            if (!startMatch.Success)
                break;

            string toolName = startMatch.Groups["name"].Value;

            List<string> acceptedEndTags = new() // Allow tool endings, since only 1 tool is getting called at a time
            {
                $"</tool_call name=\"{toolName}\")>",
                "</tool_call>",
                "</tool>"
            };

            int searchStart = context.SearchPosition + startMatch.Index;

            var matches = acceptedEndTags
                .Select(tag => new { Tag = tag, Index = context.Buffer.IndexOf(tag, searchStart) })
                .Where(x => x.Index >= 0)
                .ToList();

            if (!matches.Any())
                break;

            var chosen = matches.OrderBy(x => x.Index).First();
            int endTagIndex = chosen.Index;
            string endTag = chosen.Tag;

            int blockStartIndex = searchStart + startMatch.Length;
            string innerContent = context.Buffer.Substring(blockStartIndex, endTagIndex - blockStartIndex);

            // Get the arg and value into a dict
            var args = Regex.Matches(innerContent, @"<(?<key>\w+)>(?<value>.*?)<\/\k<key>>", RegexOptions.Singleline)
                .ToDictionary(m => m.Groups["key"].Value, m => m.Groups["value"].Value.Trim());

            // Change search position to not parse 1 command twice
            context.SearchPosition = endTagIndex + endTag.Length;

            return new Command()
            {
                Tool = toolName,
                Args = args
            };
        }


        return null;
    }

    public static string UnescapeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return WebUtility.HtmlDecode(input);
    }

}

public class ParserContext
{
    public static Dictionary<string, string>? ToolOutputs;
    public string Buffer { get; set; }
    public int SearchPosition { get; set; }
    public List<Command> ToolQueue { get; set; } = new();
    public ParserContext(string buffer, int searchpos)
    {
        Buffer = buffer;
        SearchPosition = searchpos;
    }
}

public class Command
{
    public string Thought { get; set; } = string.Empty;
    public string Tool { get; set; } = string.Empty;
    public Dictionary<string, string> Args { get; set; } = null!;
    public string Error { get; set; }
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Tool: {Tool}");
        sb.AppendLine($"Thought: {Thought}");
        sb.AppendLine("Args:");
        foreach (var item in Args)
            sb.AppendLine($"  {item.Key}: {item.Value}");

        return sb.ToString();
    }
}
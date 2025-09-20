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
            var startMatch = Regex.Match(context.Buffer.Substring(context.SearchPosition), @"<toolcall\((?<name>.*?)\)>");
            if (!startMatch.Success) break;

            string toolName = startMatch.Groups["name"].Value;

            string endTagSpecific = $"</toolcall({toolName})>";
            string endTagGeneric = "</toolcall>";

            int searchStart = context.SearchPosition + startMatch.Index;
            int endTagSpecificIndex = context.Buffer.IndexOf(endTagSpecific, searchStart);
            int endTagGenericIndex = context.Buffer.IndexOf(endTagGeneric, searchStart);

            int endTagIndex;
            string endTag;
            if (endTagSpecificIndex != -1 && (endTagGenericIndex == -1 || endTagSpecificIndex < endTagGenericIndex))
            {
                endTagIndex = endTagSpecificIndex;
                endTag = endTagSpecific;
            }
            else if (endTagGenericIndex != -1)
            {
                endTagIndex = endTagGenericIndex;
                endTag = endTagGeneric;
            }
            else
                break;

            int blockStartIndex = searchStart + startMatch.Length;
            string innerContent = context.Buffer.Substring(blockStartIndex, endTagIndex - blockStartIndex);

            var args = Regex.Matches(innerContent, @"<(?<key>\w+)>(?<value>.*?)<\/\k<key>>", RegexOptions.Singleline)
                .ToDictionary(m => m.Groups["key"].Value, m => m.Groups["value"].Value.Trim());

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
    public static string ToolOutputs = "";
    public string Buffer { get; set; }
    public int SearchPosition { get; set; }
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
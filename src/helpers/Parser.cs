// src/helpers/Parser.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using static AISlop.Parser;

namespace AISlop
{
    public class Parser
    {
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

        public class ToolCall
        {
            [JsonPropertyName("tool")]
            public string Tool { get; set; } = string.Empty;

            [JsonPropertyName("args")]
            public Dictionary<string, string> Args { get; set; } = null!;
        }

        public class AIResponse
        {
            [JsonPropertyName("thought")]
            public string Thought { get; set; } = string.Empty;

            [JsonPropertyName("tool_calls")]
            public IEnumerable<ToolCall> ToolCalls { get; set; } = null!;
        }

        public static IEnumerable<Command> Parse(string response)
        {
            string jsonCommand = ExtractJson(response);
            if (jsonCommand.StartsWith("Exception:"))
                return new List<Command>() { new() { Error = jsonCommand } };

            try
            {
                var aiResponse = JsonSerializer.Deserialize<AIResponse>(jsonCommand);
                if (aiResponse?.ToolCalls?.Count() is null or 0)
                    return new List<Command>() { new() { Error = "Exception: No jsons found in the response!" } };

                return aiResponse.ToolCalls
                    .Select(tc => new Command
                    {
                        Thought = aiResponse.Thought,
                        Tool = tc.Tool,
                        Args = tc.Args
                    })
                    .ToList();
            }
            catch (JsonException ex)
            {
                return new List<Command>() { new() { Error = ex.Message } };
            }
        }

        public static string ExtractJson(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
                return "Exception: Response was empty!";

            int braceCount = 0;
            bool inString = false;
            char stringChar = '\0';
            int startIndex = -1;

            for (int i = 0; i < rawResponse.Length; i++)
            {
                char c = rawResponse[i];

                if ((c == '"' || c == '\'') && (i == 0 || rawResponse[i - 1] != '\\'))
                {
                    if (!inString)
                    {
                        inString = true;
                        stringChar = c;
                    }
                    else if (c == stringChar)
                        inString = false;
                }

                if (!inString)
                {
                    if (c == '{')
                    {
                        if (braceCount == 0)
                            startIndex = i;
                        braceCount++;
                    }
                    else if (c == '}')
                    {
                        braceCount--;
                        if (braceCount == 0 && startIndex != -1)
                        {
                            string candidate = rawResponse.Substring(startIndex, i - startIndex + 1);
                            try
                            {
                                JsonDocument.Parse(candidate);
                                return candidate;
                            }
                            catch (JsonException)
                            {
                                return "Exception: The first JSON-like structure found was invalid.";
                            }
                        }
                    }
                }
            }

            return "Exception: No valid JSON found in the response!";
        }
    }
}

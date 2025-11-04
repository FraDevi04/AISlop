// src/tools/gmail/ReadEmail.cs
using Google.Apis.Gmail.v1;
using System.Text;

namespace AISlop;

public class ReadEmail : ITool
{
    public string Name => "read_email";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GmailHelper.GetServiceAsync();
            string id = args.GetValueOrDefault("id");
            if (string.IsNullOrWhiteSpace(id)) return "Error: 'id' parameter is required.";

            var msg = await service.Users.Messages.Get("me", id).ExecuteAsync();

            var sb = new StringBuilder();
            sb.AppendLine($"ID: {msg.Id}");
            var headers = msg.Payload?.Headers;
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    if (h.Name == "From" || h.Name == "To" || h.Name == "Subject" || h.Name == "Date")
                        sb.AppendLine($"{h.Name}: {h.Value}");
                }
            }

            // Basic body extraction (plain text parts)
            string? body = ExtractPlainText(msg.Payload);
            if (!string.IsNullOrEmpty(body))
            {
                sb.AppendLine();
                sb.AppendLine(body);
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error reading email: {ex.Message}";
        }
    }

    private string? ExtractPlainText(Google.Apis.Gmail.v1.Data.MessagePart part)
    {
        if (part == null) return null;
        if (part.MimeType == "text/plain" && part.Body?.Data != null)
        {
            var data = part.Body.Data.Replace('-', '+').Replace('_', '/');
            var bytes = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(bytes);
        }
        if (part.Parts != null)
        {
            foreach (var p in part.Parts)
            {
                var txt = ExtractPlainText(p);
                if (!string.IsNullOrEmpty(txt)) return txt;
            }
        }
        return null;
    }
}



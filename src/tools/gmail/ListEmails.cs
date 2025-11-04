// src/tools/gmail/ListEmails.cs
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Text;

namespace AISlop;

public class ListEmails : ITool
{
    public string Name => "list_emails";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GmailHelper.GetServiceAsync();

            string query = args.GetValueOrDefault("query") ?? string.Empty; // Gmail search query
            int maxResults = 10;
            if (args.TryGetValue("max_results", out var mr) && int.TryParse(mr, out var mrInt)) maxResults = mrInt;

            var request = service.Users.Messages.List("me");
            request.Q = query;
            request.MaxResults = maxResults;
            var response = await request.ExecuteAsync();

            if (response.Messages == null || response.Messages.Count == 0)
                return "No emails found.";

            var sb = new StringBuilder();
            sb.AppendLine($"Found {response.Messages.Count} email(s):\n");

            foreach (var msgRef in response.Messages)
            {
                var msg = await service.Users.Messages.Get("me", msgRef.Id).ExecuteAsync();
                var subject = msg.Payload?.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "(no subject)";
                var from = msg.Payload?.Headers?.FirstOrDefault(h => h.Name == "From")?.Value ?? "(unknown)";
                var date = msg.Payload?.Headers?.FirstOrDefault(h => h.Name == "Date")?.Value ?? "";
                sb.AppendLine($"ID: {msg.Id}");
                sb.AppendLine($"From: {from}");
                sb.AppendLine($"Subject: {subject}");
                sb.AppendLine($"Date: {date}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error listing emails: {ex.Message}";
        }
    }
}



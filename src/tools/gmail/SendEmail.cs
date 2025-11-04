// src/tools/gmail/SendEmail.cs
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace AISlop;

public class SendEmail : ITool
{
    public string Name => "send_email";

    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        try
        {
            var service = await GmailHelper.GetServiceAsync();

            string to = args.GetValueOrDefault("to");
            string subject = args.GetValueOrDefault("subject") ?? "(no subject)";
            string body = args.GetValueOrDefault("body") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(to))
                return "Error: 'to' parameter is required.";

            var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            var mimeMessage = MimeMessageToRaw(message);
            var gmailMessage = new Message { Raw = mimeMessage };

            var request = service.Users.Messages.Send(gmailMessage, "me");
            var result = await request.ExecuteAsync();

            return $"Email sent. ID: {result.Id}";
        }
        catch (Exception ex)
        {
            return $"Error sending email: {ex.Message}";
        }
    }

    private string MimeMessageToRaw(MailMessage message)
    {
        using var client = new SmtpClient();
        using var mmStream = new MemoryStream();
        var altView = AlternateView.CreateAlternateViewFromString(message.Body, Encoding.UTF8, MediaTypeNames.Text.Plain);
        message.AlternateViews.Add(altView);
        var pickup = new System.Net.Mime.ContentType("message/rfc822");
        message.HeadersEncoding = Encoding.UTF8;
        // Build MIME string manually
        var sb = new StringBuilder();
        sb.AppendLine($"To: {string.Join(", ", message.To.Select(x => x.Address))}");
        sb.AppendLine($"Subject: {message.Subject}");
        sb.AppendLine("Content-Type: text/plain; charset=utf-8");
        sb.AppendLine();
        sb.AppendLine(message.Body ?? string.Empty);
        var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()))
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", string.Empty);
        return raw;
    }
}



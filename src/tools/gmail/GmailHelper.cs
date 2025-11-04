// src/tools/gmail/GmailHelper.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AISlop;

public static class GmailHelper
{
    private static readonly string[] Scopes = new[]
    {
        GmailService.Scope.GmailSend,
        GmailService.Scope.GmailReadonly
    };

    private static GmailService _service;

    public static async Task<GmailService> GetServiceAsync()
    {
        if (_service != null) return _service;

        string credentialsPath = Config.Settings.google_calendar_credentials_json; // reuse same creds
        string tokenDir = Path.Combine(Path.GetDirectoryName(Config.Settings.google_calendar_token_json) ?? ".", "gmail_tokens");

        if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            throw new Exception("Gmail credentials file not found. Please set google_calendar_credentials_json in config.json");

        var credential = await GoogleAuthHelper.AuthorizeAsync(Scopes, "gmail_tokens");

        _service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "AISlop"
        });

        return _service;
    }
}

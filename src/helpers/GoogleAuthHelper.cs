// src/helpers/GoogleAuthHelper.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

namespace AISlop;

public static class GoogleAuthHelper
{
    public static async Task<UserCredential> AuthorizeAsync(string[] scopes, string tokenSubdir)
    {
        string credentialsPath = Config.Settings.google_calendar_credentials_json;
        string tokenDir = Path.Combine(Path.GetDirectoryName(Config.Settings.google_calendar_token_json) ?? ".", tokenSubdir);

        if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            throw new Exception("Google credentials file not found. Please set google_calendar_credentials_json in config.json");

        using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
        var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = scopes,
            DataStore = new FileDataStore(tokenDir, true)
        });

        var receiver = new LocalServerCodeReceiver();
        var redirectUri = receiver.RedirectUri;
        var authUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build().AbsoluteUri;

        Console.WriteLine("\n[OAuth] Open this URL to authorize if browser did not open:\n" + authUrl + "\n");
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });
        }
        catch {}

        var codeResponse = await receiver.ReceiveCodeAsync(flow.CreateAuthorizationCodeRequest(redirectUri), CancellationToken.None);
        var token = await flow.ExchangeCodeForTokenAsync("user", codeResponse.Code, redirectUri, CancellationToken.None);
        return new UserCredential(flow, "user", token);
    }
}

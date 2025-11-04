// src/tools/googlecalendar/GoogleCalendarHelper.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AISlop;

public static class GoogleCalendarHelper
{
    private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
    private static CalendarService? _service;

    public static async Task<CalendarService> GetServiceAsync()
    {
        if (_service != null)
            return _service;

        try
        {
            string credentialsPath = Config.Settings.google_calendar_credentials_json;
            string tokenPath = Config.Settings.google_calendar_token_json;

            if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            {
                throw new Exception("Google Calendar credentials file not found. Please set google_calendar_credentials_json in config.json");
            }

            var credential = await GoogleAuthHelper.AuthorizeAsync(Scopes, "calendar_tokens");

            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AISlop",
            });

            return _service;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to initialize Google Calendar service: {ex.Message}");
        }
    }
}

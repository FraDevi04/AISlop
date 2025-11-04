// src/helpers/Config.cs
using System.Text.Json;

namespace AISlop;

public class Settings
{
    public string model_name { get; set; } = string.Empty;
    public bool generate_log { get; set; }
    public bool display_thought { get; set; }
    public bool display_toolcall { get; set; }
    public string api_url { get; set; } = string.Empty;
    public string api_key { get; set; } = string.Empty;
    public string search_api_key { get; set; } = string.Empty;
    public string search_engine_id { get; set; } = string.Empty;
    public string google_calendar_credentials_json { get; set; } = string.Empty;
    public string google_calendar_token_json { get; set; } = string.Empty;
    public string notion_api_key { get; set; } = string.Empty;

}
public static class Config
{
    private static string configPath;
    private static Settings _settings = null!;

    static Config()
    {
        string solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        string configDir = Path.Combine(solutionDir, "config");

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        configPath = Path.Combine(configDir, "config.json");
    }

    public static Settings Settings
    {
        get
        {
            if (_settings == null) LoadConfig();
            return _settings;
        }
    }

    public static void LoadConfig()
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            _settings = JsonSerializer.Deserialize<Settings>(json);
        }
        else
        {
            _settings = new Settings
            {
                model_name = "YOUR_AI_MODEL",
                generate_log = false,
                display_thought = false,
                display_toolcall = false,
                api_url = "http://localhost:11434",
                api_key = "YOUR_API_KEY",
                search_api_key = "YOUR_GOOGLE_SEARCH_API_KEY",
                search_engine_id = "YOUR_GOOGLE_SEARCH_ID",
                google_calendar_credentials_json = "YOUR_GOOGLE_CALENDAR_CREDENTIALS_JSON_PATH",
                google_calendar_token_json = "YOUR_GOOGLE_CALENDAR_TOKEN_JSON_PATH",
                notion_api_key = "YOUR_NOTION_API_KEY"
            };
            Console.WriteLine("Please change the default config file in the configs folder, then relaunch the app");
            SaveConfig();
        }
    }

    public static void SaveConfig()
    {
        string json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
    }
}

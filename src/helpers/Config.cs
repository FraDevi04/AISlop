// src/helpers/Config.cs
using System.Diagnostics;
using System.Text.Json;
using System.Xml;

namespace AISlop;

public class Settings
{
    public string model_name { get; set; }
    public bool generate_log { get; set; }
    public bool display_thought { get; set; }
    public bool display_toolcall { get; set; }
    public string api_url { get; set; }
    public string api_key { get; set; }
    public string search_api_key { get; set; }

}
public static class Config
{
    private static string configPath;
    private static Settings _settings;

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
                search_api_key = "YOUR_GOOGLE_SEARCH_API_KEY"
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

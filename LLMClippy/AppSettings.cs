using Microsoft.Extensions.Configuration;

namespace LLMClippy
{
    public static class AppSettings
    {
        private static IConfigurationRoot Config => Program.Configuration;

        public static IConfigurationSection AzureOpenAISettings =>
            Config.GetSection("AzureOpenAISettings");

        public static IEnumerable<string> GetAzureModelNames()
        {
            return AzureOpenAISettings.GetChildren().Select(section => section.Key);
        }

        public static IEnumerable<string> GetSystemPrompts()
        {
            // Fix for CS8619: Ensure null values are filtered out before returning the IEnumerable<string>
            return Config.AsEnumerable()
                .Where(kv => kv.Key.StartsWith("SystemPrompt", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(kv.Value))
                .Select(kv => kv.Value!)
                .Where(value => value != null); // Additional null check for safety
        }

        public static IConfigurationSection GetModelConfig(string modelName)
        {
            return AzureOpenAISettings.GetSection(modelName);
        }
    }
}
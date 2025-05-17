using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Specialized;
using System.Configuration;

namespace LLMClippy
{
    internal class AOAI
    {
        private readonly AzureOpenAIClient client;
        private readonly ChatClient chatClient;
        private string systemMessage;

        public AOAI(string systemMessage, string configSetName = "GPT41")
        {
            // Retrieve the endpoint, API key environment variable name, and deployment name from app.config
            var azureOpenAISettings = (NameValueCollection)ConfigurationManager.GetSection("AzureOpenAISettings");

            string endpoint = azureOpenAISettings[$"{configSetName}.Endpoint"];
            string apiKeyEnvVar = azureOpenAISettings[$"{configSetName}.ApiKeyEnvVar"];
            string deploymentName = azureOpenAISettings[$"{configSetName}.DeploymentName"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKeyEnvVar) || string.IsNullOrEmpty(deploymentName))
            {
                throw new InvalidOperationException($"Configuration for {configSetName} is missing or incomplete.");
            }

            string apiKey = Environment.GetEnvironmentVariable(apiKeyEnvVar);
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException($"Environment variable {apiKeyEnvVar} is not set or empty.");
            }

            // Initialize AzureOpenAIClient and ChatClient
            client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
            chatClient = client.GetChatClient(deploymentName);
            this.systemMessage = systemMessage;
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(this.systemMessage),
                new UserChatMessage(prompt) // Use the provided prompt
            };

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions()
            {
                Temperature = 0.7f,
                FrequencyPenalty = 0f,
                PresencePenalty = 0f,
                MaxOutputTokenCount = 4096,
            });

            var chatResponse = response.Value.Content.Last().Text;

            return chatResponse; // Ensure a value is returned
        }
    }
}

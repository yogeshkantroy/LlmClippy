using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LLMClippy
{
    internal class Assistant
    {
        private readonly AzureOpenAIClient client;
        private readonly ChatClient chatClient;
        private readonly string systemMessage;

        public Assistant(string systemMessage, string configSetName = "GPT45")
        {
            this.systemMessage = systemMessage;

            // Retrieve configuration from app settings
            var azureOpenAISettings = AppSettings.AzureOpenAISettings;
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
        }

   }
}
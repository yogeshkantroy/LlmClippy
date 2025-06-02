using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections.Specialized;
using System.Configuration;
using HtmlAgilityPack;

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
            var modelConfig = AppSettings.GetModelConfig(configSetName);

            string endpoint = modelConfig["Endpoint"];
            string apiKeyEnvVar = modelConfig["ApiKeyEnvVar"];
            string deploymentName = modelConfig["DeploymentName"];

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

            return await GetResponseAsync(messages);
        }

        public async Task<string> AnalyzeImage(BinaryData imageBytes, string imageBytesMediaType, string context = "")
        {
            var imagePart = ChatMessageContentPart.CreateImagePart(imageBytes, imageBytesMediaType);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(this.systemMessage),
                new UserChatMessage(context + Environment.NewLine),
                new UserChatMessage(imagePart), 
                new UserChatMessage("Explain the above image in detail starting with Image:<image caption>")
            };

            return await GetResponseAsync(messages);
        }


        public async Task<string> GetResponseAsync(List<ChatMessage> messages)
        {
            var response = await chatClient.CompleteChatAsync(messages);

            var chatResponse = response.Value.Content.Last().Text;

            return chatResponse;
        }

        /// <summary>
        /// Creates a UserChatMessage from HTML input, extracting all text and image URLs.
        /// </summary>
        /// <param name="html">The HTML string to parse.</param>
        /// <returns>A ChatMessage containing the extracted text and image links.</returns>
        public static ChatMessage CreateChatMessageFromHtml(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Helper to recursively extract text and images in order
            static void ExtractContent(HtmlNode node, List<string> parts)
            {
                foreach (var child in node.ChildNodes)
                {
                    if (child.NodeType == HtmlNodeType.Text)
                    {
                        var text = child.InnerText;
                        if (!string.IsNullOrWhiteSpace(text))
                            parts.Add(text.Trim());
                    }
                    else if (child.Name.Equals("img", StringComparison.OrdinalIgnoreCase))
                    {
                        var src = child.GetAttributeValue("src", null);
                        if (!string.IsNullOrEmpty(src))
                            parts.Add($"[Image]({src})");
                    }
                    else
                    {
                        ExtractContent(child, parts);
                    }
                }
            }

            var contentParts = new List<string>();
            ExtractContent(doc.DocumentNode, contentParts);

            string content = string.Join("\n", contentParts);
            return new UserChatMessage(content);
        }
    }
}

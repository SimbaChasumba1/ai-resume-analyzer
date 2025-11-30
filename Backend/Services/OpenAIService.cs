using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
    public class OpenAIService
    {
        private readonly OpenAIClient _client;

        public OpenAIService(IConfiguration config)
        {
            var apiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("OpenAI API key is missing in configuration.");

            _client = new OpenAIClient(new OpenAIAuthentication(apiKey));
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText)
        {
            var chatRequest = new ChatRequest()
            {
                Model = Models.Gpt3_5Turbo,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.User, resumeText)
                }
            };

            var response = await _client.ChatEndpoint.GetCompletionAsync(chatRequest);

            return response.Choices[0].Message.Content;
        }
    }
}

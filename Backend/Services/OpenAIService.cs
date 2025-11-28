using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Backend.Services
{
    public class OpenAIService
    {
        private readonly OpenAIClient _client;

        public OpenAIService(IConfiguration config)
        {
            _client = new OpenAIClient(config["OPENAI_API_KEY"]);
        }

        public async Task<string> AnalyzeResume(string resumeText)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, "You are an expert career advisor."),
                new ChatMessage(ChatMessageRole.User, resumeText)
            };

            var request = new ChatCompletionCreateRequest
            {
                Model = Models.Gpt35Turbo,
                Messages = messages
            };

            var response = await _client.ChatCompletions.CreateCompletionAsync(request);
            return response.Choices[0].Message.Content;
        }
    }
}

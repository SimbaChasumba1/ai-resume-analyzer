using Azure.AI.OpenAI;
using backend.Models; // only if you have your own models

namespace backend.Services
{
    public class OpenAIService
    {
        private readonly OpenAIClient _client;

        public OpenAIService()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
            _client = new OpenAIClient(apiKey);
        }

        public async Task<string> AnalyzeResume(string resumeText)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a resume analyzer."),
                    new ChatMessage(ChatRole.User, resumeText)
                }
            };

            var response = await _client.GetChatCompletionsAsync("gpt-35-turbo", chatCompletionsOptions);
            return response.Value.Choices[0].Message.Content;
        }
    }
}

using OpenAI.Chat;

namespace Backend.Services
{
    public class OpenAIService
    {
        private readonly string _apiKey;

        public OpenAIService(IConfiguration config)
        {
            _apiKey = config["OPENAI_API_KEY"];
        }

        public async Task<string> AnalyzeResume(string text)
        {
            var client = new ChatClient("gpt-4o-mini", _apiKey);

            var result = await client.CompleteAsync(
                $"Analyze this resume text and provide improvements, missing skills, strengths, weaknesses:\n\n{text}"
            );

            return result.Value;
        }
    }
}

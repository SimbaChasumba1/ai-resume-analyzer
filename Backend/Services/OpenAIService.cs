using OpenAI_API;
using OpenAI_API.Completions;

namespace backend.Services
{
    public class OpenAIService
    {
        private readonly OpenAIAPI _api;

        public OpenAIService(string apiKey)
        {
            _api = new OpenAIAPI(apiKey);
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText)
        {
            var req = new CompletionRequest
            {
                Prompt = $"Analyze this resume and summarise it briefly for a recruiter:\n\n{resumeText}",
                Model = "text-davinci-003",
                MaxTokens = 400
            };

            var result = await _api.Completions.CreateCompletionAsync(req);
            return result.Completions[0].Text.Trim();
        }
    }
}

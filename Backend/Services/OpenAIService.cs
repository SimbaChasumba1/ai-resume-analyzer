using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Completions;

namespace backend.Services
{
    public class OpenAIService
    {
        private readonly OpenAIAPI _api;

        public OpenAIService(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key is required", nameof(apiKey));

            _api = new OpenAIAPI(apiKey);
        }

        public async Task<string> AnalyzeResumeAsync(string resumeText)
        {
            if (string.IsNullOrWhiteSpace(resumeText))
                return string.Empty;

            var completionRequest = new CompletionRequest
            {
                Prompt = resumeText,
                Model = "text-davinci-003",
                MaxTokens = 500
            };

            var result = await _api.Completions.CreateCompletionAsync(completionRequest);
            return result.Completions.Count > 0 ? result.Completions[0].Text : string.Empty;
        }
    }
}

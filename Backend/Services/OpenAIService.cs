using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace backend.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _http = new();

        public async Task<string> AnalyzeResume(string resumeText)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a professional resume reviewer." },
                    new { role = "user", content = resumeText }
                }
            };

            var res = await _http.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var json = await res.Content.ReadAsStringAsync();
            return json;
        }
    }
}

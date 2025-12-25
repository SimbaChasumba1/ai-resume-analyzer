using System.Text.RegularExpressions;

namespace backend.Services
{
    public class ResumeParser
    {
        public string ParseText(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return string.Empty;

            // Normalize whitespace
            var cleaned = Regex.Replace(rawText, @"\s+", " ").Trim();
            return cleaned;
        }
    }
}

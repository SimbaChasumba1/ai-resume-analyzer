using System.Collections.Generic;

namespace backend.Services
{
    public class SkillExtractor
    {
        public List<string> Extract(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            // Simple stub: split by common delimiters
            var skills = text.Split(new[] { ',', '.', ';', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(skills);
        }
    }
}

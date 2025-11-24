using System.Text.RegularExpressions;

namespace Backend.Services
{
    public class SkillExtractor
    {
        private readonly string[] skills = new[] {
            "C#","C++","Python","JavaScript","TypeScript","React","Next.js","Node.js","SQL","PostgreSQL","MongoDB",
            "Docker","Kubernetes","Azure","AWS","GCP","ASP.NET","Entity Framework","REST","GraphQL","HTML","CSS","Sass",
            "Redis","RabbitMQ","Kafka","NUnit","xUnit","Jest","Mocha","Tailwind","Figma"
        };

        public List<string> Extract(string text)
        {
            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(text)) return found.ToList();

            foreach (var skill in skills)
            {
                var pattern = $@"\b{Regex.Escape(skill)}\b";
                if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                    found.Add(skill);
            }

            return found.ToList();
        }
    }
}

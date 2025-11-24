using System.IO;
using Xceed.Words.NET;
using UglyToad.PdfPig;

namespace Backend.Services
{
    public class ResumeParser
    {
        private readonly SkillExtractor _skillExtractor;

        public ResumeParser(SkillExtractor skillExtractor)
        {
            _skillExtractor = skillExtractor;
        }

        public List<string> ExtractSkillsFromResume(string filePath)
        {
            var text = string.Empty;
            if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (var doc = PdfDocument.Open(filePath))
                {
                    foreach (var page in doc.GetPages()) text += "\n" + page.Text;
                }
            }
            else if (filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                var doc = DocX.Load(filePath);
                text = doc.Text ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(text))
                text = File.ReadAllText(filePath);

            return _skillExtractor.Extract(text);
        }
    }
}

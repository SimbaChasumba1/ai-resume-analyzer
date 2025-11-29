using System;
using System.Collections.Generic;
using System.IO;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace backend.Services
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
            string text = string.Empty;

            if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var doc = PdfDocument.Open(filePath);
                foreach (var page in doc.GetPages())
                    text += "\n" + page.Text;
            }
            else if (filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                using WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false);
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body != null)
                    text = body.InnerText;
            }
            else
            {
                text = File.ReadAllText(filePath);
            }

            if (string.IsNullOrWhiteSpace(text))
                text = File.ReadAllText(filePath);

            return _skillExtractor.Extract(text);
        }
    }
}

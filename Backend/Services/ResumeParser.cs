using System.Collections.Generic;
using System.IO;
using Xceed.Words.NET; // For DOCX parsing
using UglyToad.PdfPig; // For PDF parsing

public class ResumeParser
{
    // Extracts skills from the uploaded resume
    public List<string> ExtractSkillsFromResume(string filePath)
    {
        var extractedSkills = new List<string>();

        // Check if it's a PDF or DOCX file and process accordingly
        if (filePath.EndsWith(".pdf"))
        {
            var text = PdfReader.ReadText(filePath); // Read PDF text
            extractedSkills.AddRange(ExtractSkillsFromText(text)); // Extract skills from text
        }
        else if (filePath.EndsWith(".docx"))
        {
            var doc = DocX.Load(filePath); // Load DOCX file
            var text = doc.Text;
            extractedSkills.AddRange(ExtractSkillsFromText(text)); // Extract skills from text
        }

        return extractedSkills;
    }

    // Simple skill extraction (In real-world scenarios, you would improve this with NLP or regex)
    private List<string> ExtractSkillsFromText(string text)
    {
        var skills = new List<string> { "C#", "JavaScript", "React", "SQL" };
        return skills; // This is a mock implementation, adjust as needed
    }
}

using System.Text;
using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace Backend.Services {
    
public class ResumeParser
{
    public string ExtractText(string filePath)
    {
        if (filePath.EndsWith(".pdf"))
        {
            var sb = new StringBuilder();
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            return sb.ToString();
        }
        else if (filePath.EndsWith(".docx"))
        {
            using var doc = DocX.Load(filePath);
            return doc.Text;
        }
        else
        {
            throw new Exception("Unsupported file type");
        }
    }
}
}
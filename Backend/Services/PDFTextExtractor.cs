using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace backend.Services
{
    public class PDFTextExtractor
    {
        public string ExtractText(string filePath)
        {
            using var document = PdfDocument.Open(filePath);
            var text = "";

            foreach (Page page in document.GetPages())
            {
                text += page.Text + "\n";
            }

            return text;
        }
    }
}

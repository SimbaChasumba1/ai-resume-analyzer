using System.IO;
using UglyToad.PdfPig;

namespace backend.Services
{
    public class PdfTextExtractor : IPdfTextExtractor
    {
        public string ExtractText(Stream pdfStream)
        {
            try
            {
                using var document = PdfDocument.Open(pdfStream);
                var text = "";
                foreach (var page in document.GetPages())
                {
                    text += page.Text + "\n";
                }

                return string.IsNullOrWhiteSpace(text) ? "" : text.Trim();
            }
            catch
            {
                return ""; // Return empty if extraction fails
            }
        }
    }
}

using UglyToad.PdfPig;
using backend.Services;

namespace backend.Services;

public interface IPdfTextExtractor
{
    string ExtractText(Stream pdfStream);
}

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

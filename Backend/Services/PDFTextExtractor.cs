using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace backend.Services
{
    public class PDFTextExtractor
    {
        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            string text = "";
            using (var pdf = PdfDocument.Open(ms))
            {
                foreach (var page in pdf.GetPages())
                    text += page.Text + "\n";
            }

            return text;
        }
    }
}

using System.IO;

namespace backend.Services
{
    public interface IPdfTextExtractor
    {
        string ExtractText(Stream pdfStream);
    }
}

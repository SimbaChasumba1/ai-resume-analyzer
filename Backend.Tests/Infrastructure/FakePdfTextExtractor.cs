using backend.Services;

namespace Backend.Tests.Infrastructure;

public class FakePdfTextExtractor : IPdfTextExtractor
{
    public string ExtractText(Stream pdfStream)
    {
        return "Software Developer with C#, ASP.NET Core, SQL, and Azure experience.";
    }
}
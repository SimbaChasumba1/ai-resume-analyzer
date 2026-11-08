using System.Text;
using backend.Services;

namespace Backend.Tests.Services;

public class PDFTextExtractorTests
{
    private readonly PdfTextExtractor _extractor = new();

    [Fact]
    public void ExtractText_ValidPdf_ReturnsExtractedText()
    {
        // Arrange
        using var stream = CreatePdf("Resume text extraction works");

        // Act
        var result = _extractor.ExtractText(stream);

        // Assert
        Assert.Contains("Resume text extraction works", result);
    }

    [Fact]
    public void ExtractText_EmptyStream_ReturnsEmptyString()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = _extractor.ExtractText(stream);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ExtractText_InvalidPdf_ReturnsEmptyString()
    {
        // Arrange
        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes("This is not a valid PDF")
        );

        // Act
        var result = _extractor.ExtractText(stream);

        // Assert
        Assert.Equal("", result);
    }

    private static MemoryStream CreatePdf(string text)
    {
        var content = $"BT /F1 18 Tf 50 100 Td ({text}) Tj ET";
        var contentBytes = Encoding.ASCII.GetBytes(content);

        var objects = new[]
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 300 300] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
            $"<< /Length {contentBytes.Length} >>\nstream\n{content}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        };

        var pdf = new StringBuilder();
        pdf.Append("%PDF-1.4\n");

        var offsets = new List<int> { 0 };

        for (var i = 0; i < objects.Length; i++)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));

            pdf.Append($"{i + 1} 0 obj\n");
            pdf.Append(objects[i]);
            pdf.Append("\nendobj\n");
        }

        var xrefOffset = Encoding.ASCII.GetByteCount(pdf.ToString());

        pdf.Append($"xref\n0 {objects.Length + 1}\n");
        pdf.Append("0000000000 65535 f \n");

        foreach (var offset in offsets.Skip(1))
        {
            pdf.Append($"{offset:D10} 00000 n \n");
        }

        pdf.Append(
            $"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\n"
        );
        pdf.Append($"startxref\n{xrefOffset}\n%%EOF");

        return new MemoryStream(Encoding.ASCII.GetBytes(pdf.ToString()));
    }
}
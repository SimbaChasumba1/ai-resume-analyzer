using UglyToad.PdfPig;



namespace backend.Services;



public interface IPdfTextExtractor

{

    string ExtractText(Stream pdfStream);

}



public class PdfTextExtractor : IPdfTextExtractor

{

    public string ExtractText(Stream pdfStream)

    {

        using var document = PdfDocument.Open(pdfStream);

        var text = "";



        foreach (var page in document.GetPages())

        {

            text += page.Text + "\n";

        }



        return text.Trim();

    }

}






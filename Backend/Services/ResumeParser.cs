namespace backend.Services
{
    public class ResumeParser
    {
        private readonly PDFTextExtractor _extractor;

        public ResumeParser(PDFTextExtractor extractor)
        {
            _extractor = extractor;
        }

        public string Parse(string filePath)
        {
            return _extractor.ExtractText(filePath);
        }
    }
}

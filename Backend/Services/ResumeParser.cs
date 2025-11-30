using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace backend.Services
{
    public class ResumeParser
    {
        public ResumeParser()
        {
        }

        /// <summary>
        /// Read file at path and return extracted plain text.
        /// Supports PDF and DOCX (OpenXML). Falls back to raw text file read.
        /// </summary>
        public string ParseText(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            try
            {
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                if (ext == ".pdf")
                {
                    var sb = new StringBuilder();
                    using (var doc = PdfDocument.Open(filePath))
                    {
                        foreach (var page in doc.GetPages())
                            sb.AppendLine(page.Text);
                    }
                    return sb.ToString();
                }
                else if (ext == ".docx")
                {
                    var sb = new StringBuilder();
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                    {
                        var body = wordDoc.MainDocumentPart?.Document?.Body;
                        if (body != null)
                        {
                            sb.AppendLine(body.InnerText);
                        }
                    }
                    return sb.ToString();
                }
                else
                {
                    // fallback (txt etc)
                    return File.ReadAllText(filePath);
                }
            }
            catch
            {
                // on any parse error, return raw file contents as fallback
                try { return File.ReadAllText(filePath); } catch { return string.Empty; }
            }
        }
    }
}

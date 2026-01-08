"use client";

import { useEffect, useState } from "react";
import { Document, Page, pdfjs } from "react-pdf";

interface Props {
  url: string;
}

export default function ResumePreview({ url }: Props) {
  const [numPages, setNumPages] = useState<number | null>(null);

  useEffect(() => {
    // Set the worker from the static file in public
   pdfjs.GlobalWorkerOptions.workerSrc = "/pdf.worker.min.mjs";
  }, []);

  if (!url) return null;

  return (
    <div className="bg-white/5 rounded-xl p-4">
      <Document file={url} onLoadSuccess={({ numPages }) => setNumPages(numPages)}>
        {Array.from({ length: numPages || 0 }, (_, i) => (
          <Page key={i} pageNumber={i + 1} width={360} />
        ))}
      </Document>
    </div>
  );
}

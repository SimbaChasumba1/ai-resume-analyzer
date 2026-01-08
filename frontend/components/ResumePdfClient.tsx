"use client";

import { useEffect, useState } from "react";

type Props = {
  file: string;
  width?: number;
};

export default function ResumePdfClient({ file, width = 360 }: Props) {
  const [reactPdf, setReactPdf] = useState<any>(null);
  const [pages, setPages] = useState(1);
  const [page, setPage] = useState(1);

  useEffect(() => {
    let mounted = true;

    (async () => {
      const mod = await import("react-pdf");
      const { pdfjs } = mod;

      pdfjs.GlobalWorkerOptions.workerSrc =
        `https://cdnjs.cloudflare.com/ajax/libs/pdf.js/${pdfjs.version}/pdf.worker.min.js`;

      if (mounted) setReactPdf(mod);
    })();

    return () => {
      mounted = false;
    };
  }, []);

  if (!reactPdf) return <div>Loading PDF…</div>;

  const { Document, Page } = reactPdf;

  return (
    <div className="bg-white rounded-xl p-4">
      <Document
        file={file}
        onLoadSuccess={(pdf: any) => setPages(pdf.numPages)}
      >
        <Page pageNumber={page} width={width} />
      </Document>

      {pages > 1 && (
        <div className="flex justify-between mt-3 text-sm">
          <button disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
            Prev
          </button>
          <span>{page} / {pages}</span>
          <button disabled={page >= pages} onClick={() => setPage(p => p + 1)}>
            Next
          </button>
        </div>
      )}
    </div>
  );
}

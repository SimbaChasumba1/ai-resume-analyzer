"use client";



import { Document, Page, pdfjs } from "react-pdf";

import { useState } from "react";



pdfjs.GlobalWorkerOptions.workerSrc =

  `//cdnjs.cloudflare.com/ajax/libs/pdf.js/${pdfjs.version}/pdf.worker.js`;



export default function ResumePreview({ url }: { url: string }) {

  const [pages, setPages] = useState<number>();



  return (

    <div className="bg-black/30 border border-white/10 rounded-2xl p-4">

      <Document

        file={url}

        onLoadSuccess={(pdf) => setPages(pdf.numPages)}

      >

        <Page pageNumber={1} width={420} />

      </Document>



      <p className="text-xs text-slate-400 mt-2 text-center">

        Previewing page 1 of {pages}

      </p>

    </div>

  );

}
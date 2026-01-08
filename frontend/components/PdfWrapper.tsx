import { Document, Page } from 'react-pdf';
import { useState } from 'react';

interface PdfViewerProps {
  file: string;
}

export const PdfViewer = ({ file }: PdfViewerProps) => {
  const [numPages, setNumPages] = useState<number>(0);
  const [pageNumber, setPageNumber] = useState<number>(1);

  const onLoadSuccess = ({ numPages }: any) => {
    setNumPages(numPages);
  };

  return (
    <div className="pdf-viewer">
      <Document
        file={file}
        onLoadSuccess={onLoadSuccess}
        className="w-full"
      >
        <Page pageNumber={pageNumber} />
      </Document>

      <div className="flex justify-between items-center mt-4">
        <button
          onClick={() => setPageNumber(prev => Math.max(prev - 1, 1))}
          className="bg-indigo-600 text-white py-2 px-4 rounded"
        >
          Prev
        </button>
        <span className="text-white">{`Page ${pageNumber} of ${numPages}`}</span>
        <button
          onClick={() => setPageNumber(prev => Math.min(prev + 1, numPages))}
          className="bg-indigo-600 text-white py-2 px-4 rounded"
        >
          Next
        </button>
      </div>
    </div>
  );
};

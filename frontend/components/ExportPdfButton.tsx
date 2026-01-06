"use client";



import jsPDF from "jspdf";



export default function ExportPdfButton({ data }: any) {

  const exportPdf = () => {

    const pdf = new jsPDF();

    pdf.text(`Resume Analysis`, 10, 10);

    pdf.text(`Score: ${data.atsScore}%`, 10, 20);

    pdf.text(`Summary: ${data.summary}`, 10, 30);

    pdf.save("analysis.pdf");

  };



  return (

    <button

      onClick={exportPdf}

      className="w-full bg-indigo-600 hover:bg-indigo-500 py-3 rounded-xl font-semibold"

    >

      Export PDF

    </button>

  );

}






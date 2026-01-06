import { jsPDF } from "jspdf";

interface AnalysisResult {
  resumeFileName: string;
  summary: string;
  atsScore: number;
  strengths: string[];
  weaknesses: string[];
  improvements: string[];
}

export function exportAnalysis(analysis: AnalysisResult) {
  const doc = new jsPDF();

  doc.setFontSize(18);
  doc.text("Resume Analysis", 14, 20);

  doc.setFontSize(12);
  doc.text(`File: ${analysis.resumeFileName}`, 14, 30);
  doc.text(`ATS Score: ${analysis.atsScore}%`, 14, 40);

  doc.text("Summary:", 14, 55);
  doc.text(analysis.summary, 14, 63, { maxWidth: 180 });

  let y = 90;

  const section = (title: string, items: string[]) => {
    doc.text(title, 14, y);
    y += 8;
    items.forEach((i) => {
      doc.text(`• ${i}`, 18, y);
      y += 6;
    });
    y += 6;
  };

  section("Strengths", analysis.strengths);
  section("Weaknesses", analysis.weaknesses);
  section("Improvements", analysis.improvements);

  doc.save("resume-analysis.pdf");
}

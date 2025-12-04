import "./globals.css";

export const metadata = {
  title: "AI Resume Analyzer",
  description: "Analyze your resume with AI instantly.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="antialiased">{children}</body>
    </html>
  );
}

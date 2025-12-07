import "./globals.css";
import Navbar from "@/components/Navbar";
import React from "react";

export const metadata = {
  title: "AIResumeAnalyzer",
  description: "Upload and analyze resumes with AI — polished portfolio demo",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen bg-gradient-to-br from-[#05060a] via-[#081025] to-[#0a1220] text-slate-100 antialiased">
        <Navbar />
        <main className="max-w-6xl mx-auto px-6 py-12">{children}</main>
      </body>
    </html>
  );
}


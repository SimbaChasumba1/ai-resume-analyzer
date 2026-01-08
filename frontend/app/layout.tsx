import React from "react";
import Navbar from "@/components/Navbar";
import "./globals.css";

export default function Layout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="min-h-screen flex flex-col bg-gray-900 text-white">
        {/* Global Navbar */}
        <Navbar />
        <main className="flex-1">
          {/* Main content area */}
          {children}
        </main>
      </body>
    </html>
  );
}

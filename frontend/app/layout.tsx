import "./globals.css";
import Link from "next/link";

export const metadata = {
  title: "AIResumeAnalyzer",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="bg-gray-900 text-white min-h-screen">
        <header className="flex items-center justify-between px-8 py-4 bg-gray-800 shadow-md">
          <Link href="/">
            <h1 className="text-2xl font-bold text-indigo-400">AIResume</h1>
          </Link>
          <nav className="space-x-6">
            <Link href="/login" className="hover:text-indigo-300">
              Login
            </Link>
            <Link href="/signup" className="hover:text-indigo-300">
              Signup
            </Link>
          </nav>
        </header>
        <main>{children}</main>
      </body>
    </html>
  );
}


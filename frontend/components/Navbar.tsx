import Link from "next/link";

export default function Navbar() {
  return (
    <nav className="fixed top-0 left-0 w-full backdrop-blur-xl bg-black/40 border-b border-white/10 z-50">
      <div className="max-w-6xl mx-auto flex justify-between items-center px-6 py-4">
        
        <Link href="/" className="text-white text-2xl font-bold">
          AI Resume Analyzer
        </Link>

        <div className="flex gap-6 text-white text-lg">
          <Link href="/upload" className="hover:text-gray-300 transition">Upload</Link>
          <Link href="/auth" className="hover:text-gray-300 transition">Login</Link>
        </div>

      </div>
    </nav>
  );
}


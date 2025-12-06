"use client";

import Link from "next/link";

export default function Navbar() {
  return (
    <nav className="fixed top-0 left-0 w-full bg-[#141417]/80 backdrop-blur-lg border-b border-[#2a2a2d] z-50">
      <div className="max-w-5xl mx-auto px-6 py-4 flex items-center justify-between">

        {/* LOGO */}
        <Link href="/" className="text-2xl font-bold tracking-tight">
          <span className="text-[#6A5ACD]">AI</span>
          <span className="text-white">Resume</span>
        </Link>

        {/* BUTTONS */}
        <div className="flex gap-4">
          <Link
            href="/login"
            className="px-4 py-2 rounded-lg border border-[#6A5ACD] text-[#6A5ACD] hover:bg-[#6A5ACD] hover:text-white transition"
          >
            Login
          </Link>

          <Link
            href="/signup"
            className="px-4 py-2 rounded-lg bg-[#6A5ACD] hover:bg-[#7d6dff] transition"
          >
            Sign Up
          </Link>
        </div>
      </div>
    </nav>
  );
}


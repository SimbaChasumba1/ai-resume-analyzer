import * as React from 'react';
import Link from 'next/link';

export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="nav-left">
        <Link href="/" className="brand">AI<span className="accent">Resume</span></Link>
      </div>

      <div className="nav-right">
        <Link href="/upload" className="nav-link">Upload</Link>
        <Link href="/resume-analysis" className="nav-link">Analysis</Link>
        <Link href="/chat" className="nav-link">Chat AI</Link>
        <Link href="/auth" className="btn small">Sign in</Link>
      </div>
    </nav>
  );
}

"use client";
import Link from "next/link";

export default function Navbar() {
  return (
    <nav
      style={{
        width: "100%",
        padding: "20px 40px",
        background: "rgba(37, 99, 235, 0.15)", // blue tint
        backdropFilter: "blur(10px)",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        position: "fixed",
        top: 0,
        left: 0,
        zIndex: 50,
      }}
    >
      <Link href="/" style={{ fontSize: 22, fontWeight: 700 }}>
        AI Resume Analyzer
      </Link>

      <div style={{ display: "flex", gap: 20 }}>
        <Link href="/login">Login</Link>
        <Link href="/signup">Sign Up</Link>
      </div>
    </nav>
  );
}

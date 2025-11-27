"use client";

import Link from "next/link";

export default function Navbar() {
  return (
    <nav style={styles.nav}>
      <h2 style={styles.logo}>AI Resume Analyzer</h2>

      <div style={styles.links}>
        <Link href="/" style={styles.link}>Home</Link>
        <Link href="/resume-analysis" style={styles.link}>Analysis</Link>
        <Link href="/history" style={styles.link}>History</Link>
        <Link href="/login" style={styles.button}>Login</Link>
      </div>
    </nav>
  );
}

const styles: Record<string, any> = {
  nav: {
    position: "fixed",
    top: 0, left: 0,
    width: "100%",
    background: "white",
    padding: "15px 40px",
    display: "flex",
    justifyContent: "space-between",
    boxShadow: "0 2px 10px rgba(0,0,0,0.08)",
    zIndex: 50
  },
  logo: { fontSize: "20px", fontWeight: 700 },
  links: { display: "flex", gap: "20px", alignItems: "center" },
  link: { textDecoration: "none", color: "#222", fontWeight: 500 },
  button: {
    padding: "8px 16px",
    background: "black",
    color: "white",
    borderRadius: "6px",
    textDecoration: "none"
  }
};

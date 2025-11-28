"use client";

import Link from "next/link";
import Navbar from "../components/Navbar";

export default function HomePage() {
  return (
    <>
      <Navbar />
      <div style={{ marginTop: 120, textAlign: "center" }}>
        <h1>AI Resume Analyzer</h1>
        <p>Upload your resume and get instant AI insights.</p>
        <Link href="/upload">
          <button style={styles.button}>Go to Upload Page</button>
        </Link>
      </div>
    </>
  );
}

const styles = {
  button: {
    padding: "12px 24px",
    background: "black",
    color: "white",
    borderRadius: 8,
    fontWeight: 600,
    marginTop: 20,
    cursor: "pointer"
  }
};

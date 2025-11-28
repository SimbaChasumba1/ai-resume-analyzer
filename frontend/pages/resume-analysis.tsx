"use client";

import Navbar from "../components/Navbar";
import { useEffect, useState } from "react";

export default function AnalysisPage() {
  const [text, setText] = useState("");
  const [analysis, setAnalysis] = useState("");

  useEffect(() => {
    setText(localStorage.getItem("text") || "");
    setAnalysis(localStorage.getItem("analysis") || "");
  }, []);

  return (
    <>
      <Navbar />
      <div style={styles.container}>
        <h1 style={styles.title}>AI Resume Analysis</h1>

        <h2 style={styles.sectionTitle}>Extracted Text</h2>
        <pre style={styles.box}>{text}</pre>

        <h2 style={styles.sectionTitle}>AI Insights</h2>
        <pre style={styles.box}>{analysis}</pre>

        <div style={{ marginTop: 20 }}>
          <button style={styles.button}>Save</button>
          <button style={{ ...styles.button, marginLeft: 10 }}>New Analysis</button>
        </div>
      </div>
    </>
  );
}

const styles = {
  container: { maxWidth: 900, margin: "120px auto", padding: 20 },
  title: { fontSize: 32, marginBottom: 30 },
  sectionTitle: { marginTop: 20, fontSize: 20, fontWeight: 700 },
  box: { background: "#f8f8f8", borderRadius: 8, padding: 20, whiteSpace: "pre-wrap", marginTop: 10 },
  button: { padding: "10px 20px", background: "black", color: "white", borderRadius: 8, fontWeight: 600 }
};

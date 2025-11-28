"use client";

import { useState, useRef } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);
  const router = useRouter();

  const handleFileSelect = (f: File | null) => {
    setFile(f);
    if (f) setStatus(`Selected: ${f.name}`);
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(false);
    const f = e.dataTransfer.files[0];
    handleFileSelect(f || null);
  };

  const onUpload = async () => {
    if (!file) {
      setStatus("Please select a PDF.");
      return;
    }

    try {
      setStatus("Analyzing...");
      const formData = new FormData();
      formData.append("file", file);

      // Primary: call the AI analyze endpoint
      const res = await axios.post("http://localhost:5240/ai/analyze", formData, {
        headers: { "Content-Type": "multipart/form-data" }
      });

      // store result and navigate to analysis page
      localStorage.setItem("analysis", res.data.analysis || "");
      localStorage.setItem("text", res.data.text || "");
      router.push("/resume-analysis");
    } catch (err) {
      console.error(err);
      setStatus("Upload failed.");
    }
  };

  // Optional: raw upload (persist file to disk)
  const rawUpload = async () => {
    if (!file) return setStatus("No file selected");
    try {
      const fd = new FormData();
      fd.append("file", file);
      await axios.post("http://localhost:5240/upload", fd, { headers: { "Content-Type": "multipart/form-data" } });
      setStatus("File saved to server.");
    } catch (e) {
      console.error(e);
      setStatus("Raw upload failed.");
    }
  };

  const containerStyle: React.CSSProperties = {
    maxWidth: 600,
    margin: "120px auto 50px auto",
    background: "white",
    padding: 40,
    borderRadius: 12,
    boxShadow: "0 4px 20px rgba(0,0,0,0.07)",
    textAlign: "center"
  };

  return (
    <div style={containerStyle}>
      <h1 style={{ fontSize: 30, fontWeight: 700, marginBottom: 10 }}>Upload Your Resume</h1>

      <p style={{ fontSize: 16, color: "#555", marginBottom: 25 }}>Upload a PDF and get an AI-powered resume analysis</p>

      <div
        style={{
          border: "2px dashed #999",
          padding: 40,
          borderRadius: 12,
          cursor: "pointer",
          marginBottom: 20,
          transition: "0.2s",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          background: isDragging ? "#f0f8ff" : "#fafafa",
          borderColor: isDragging ? "#2563eb" : "#999"
        }}
        onDrop={handleDrop}
        onDragOver={(e) => { e.preventDefault(); setIsDragging(true); }}
        onDragLeave={() => setIsDragging(false)}
        onClick={() => inputRef.current?.click()}
      >
        <input ref={inputRef} type="file" accept="application/pdf" style={{ display: "none" }} onChange={(e) => handleFileSelect(e.target.files?.[0] || null)} />
        <p style={{ fontSize: 18, margin: 0 }}><strong>Click to select a PDF</strong></p>
        <p style={{ fontSize: 14, color: "#777", marginTop: 5 }}>or drag & drop here</p>
      </div>

      {file && (<p style={{ marginTop: 10, fontSize: 15 }}>📄 {file.name}</p>)}

      <div style={{ display: "flex", gap: 10, justifyContent: "center", marginTop: 15 }}>
        <button style={buttonStyle} onClick={() => inputRef.current?.click()}>Choose File</button>
        <button style={buttonStyle} onClick={onUpload} disabled={!file}>Analyze Resume</button>
        <button style={{ ...buttonStyle, background: "#666" }} onClick={rawUpload} disabled={!file}>Save File</button>
      </div>

      {status && <p style={{ marginTop: 15, fontSize: 14, color: "#333" }}>{status}</p>}
    </div>
  );
}

const buttonStyle: React.CSSProperties = {
  padding: "12px 20px",
  background: "black",
  color: "white",
  borderRadius: 8,
  cursor: "pointer",
  border: "none",
  fontWeight: 600,
  minWidth: 120
};

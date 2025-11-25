"use client";

import { useState, useRef } from "react";
import axios from "axios";
import Navbar from "../components/Navbar";

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  // Handle file selection
  const handleFileSelect = (f: File | null) => {
    setFile(f);
    if (f) setStatus(`Selected: ${f.name}`);
  };

  // Drag events
  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(false);
    const f = e.dataTransfer.files[0];
    handleFileSelect(f || null);
  };
  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(true);
  };
  const handleDragLeave = () => setIsDragging(false);

  // Upload
  const onUpload = async () => {
    if (!file) {
      setStatus("Please select a PDF.");
      return;
    }

    try {
      setStatus("Uploading...");
      const formData = new FormData();
      formData.append("file", file);

      await axios.post("http://localhost:5001/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      setStatus("Upload successful!");
    } catch {
      setStatus("Upload failed.");
    }
  };

  return (
    <>
      <Navbar />
      <div style={styles.container}>
        <h1 style={styles.title}>Upload Your Resume</h1>
        <p style={styles.subtitle}>Upload a PDF and get an AI-powered resume analysis</p>

        {/* Drag + Click Box */}
        <div
          style={{
            ...styles.uploadBox,
            borderColor: isDragging ? "#2563eb" : "#999",
            background: isDragging ? "#f0f8ff" : "#fafafa",
          }}
          onDrop={handleDrop}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onClick={() => inputRef.current?.click()}
        >
          <input
            ref={inputRef}
            type="file"
            accept="application/pdf"
            style={{ display: "none" }}
            onChange={(e) => handleFileSelect(e.target.files?.[0] || null)}
          />
          <p style={styles.uploadText}><strong>Click to select a PDF</strong></p>
          <p style={styles.uploadSubtext}>or drag & drop here</p>
        </div>

        {file && <p style={styles.selectedFile}>📄 {file.name}</p>}

        {/* Buttons */}
        <div style={styles.buttonGroup}>
          <button style={styles.button} onClick={() => inputRef.current?.click()}>
            Choose File
          </button>
          <button style={styles.button} onClick={onUpload} disabled={!file}>
            Analyze Resume
          </button>
        </div>

        {status && <p style={styles.status}>{status}</p>}
      </div>
    </>
  );
}

/* --------------------- STYLES --------------------- */

const styles: Record<string, any> = {
  container: {
    maxWidth: "600px",
    margin: "120px auto 50px auto", // space below sticky navbar
    background: "white",
    padding: "40px",
    borderRadius: "12px",
    boxShadow: "0 4px 20px rgba(0,0,0,0.07)",
    textAlign: "center",
  },
  title: { fontSize: "30px", fontWeight: 700, marginBottom: "10px" },
  subtitle: { fontSize: "16px", color: "#555", marginBottom: "25px" },
  uploadBox: {
    border: "2px dashed #999",
    padding: "40px",
    borderRadius: "12px",
    cursor: "pointer",
    marginBottom: "20px",
    transition: "0.2s",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  },
  uploadText: { fontSize: "18px", margin: 0 },
  uploadSubtext: { fontSize: "14px", color: "#777", marginTop: "5px" },
  selectedFile: { marginTop: "10px", fontSize: "15px" },
  buttonGroup: { display: "flex", gap: "10px", justifyContent: "center", marginTop: "15px" },
  button: {
    padding: "12px 20px",
    background: "black",
    color: "white",
    borderRadius: "8px",
    cursor: "pointer",
    border: "none",
    fontWeight: 600,
    minWidth: "120px",
  },
  status: { marginTop: "15px", fontSize: "14px", color: "#333" },
};

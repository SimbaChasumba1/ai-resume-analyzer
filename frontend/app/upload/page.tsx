"use client";

import { useState, useRef } from "react";
import axios from "axios";

const BACKEND_URL = "http://localhost:5240/upload";

export default function UploadPage() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  const handleFileSelect = (f: File | null) => {
    setFile(f);
    if (f) setStatus(`Selected: ${f.name}`);
  };

  const onUpload = async () => {
    if (!file) {
      setStatus("Please select a PDF.");
      return;
    }

    try {
      setStatus("Uploading...");

      const formData = new FormData();
      formData.append("file", file);

      const res = await axios.post(BACKEND_URL, formData, {
        headers: { "Content-Type": "multipart/form-data" },
        timeout: 10000
      });

      console.log("Upload response:", res.data);
      setStatus("Upload successful!");
    } catch (err) {
      console.error("UPLOAD ERROR:", err);
      setStatus("Upload failed. Backend is unreachable.");
    }
  };

  return (
    <div style={{ maxWidth: 600, margin: "120px auto", textAlign: "center" }}>
      <h1>Upload Resume</h1>

      <div
        onDrop={(e) => {
          e.preventDefault();
          setIsDragging(false);
          const f = e.dataTransfer.files[0];
          handleFileSelect(f || null);
        }}
        onDragOver={(e) => {
          e.preventDefault();
          setIsDragging(true);
        }}
        onDragLeave={() => setIsDragging(false)}
        onClick={() => inputRef.current?.click()}
        style={{
          border: "2px dashed gray",
          padding: 40,
          borderRadius: 12,
          background: isDragging ? "#eef7ff" : "#f9f9f9",
          cursor: "pointer",
        }}
      >
        <input
          type="file"
          ref={inputRef}
          style={{ display: "none" }}
          accept="application/pdf"
          onChange={(e) => handleFileSelect(e.target.files?.[0] || null)}
        />

        <p><b>Click to select a PDF</b></p>
        <p>or drag & drop here</p>
      </div>

      {file && (
        <p style={{ marginTop: 10 }}>📄 {file.name}</p>
      )}

      <button
        onClick={onUpload}
        style={{
          marginTop: 20,
          padding: "12px 22px",
          background: "black",
          color: "white",
          borderRadius: 8,
          cursor: "pointer",
        }}
      >
        Upload & Analyze
      </button>

      {status && <p style={{ marginTop: 10 }}>{status}</p>}
    </div>
  );
}

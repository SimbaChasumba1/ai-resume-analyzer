"use client";

import { useState, useRef } from "react";
import axios from "axios";
import { buttonStyle } from "@/components/styles/buttonStyle";
export default function UploadPage() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

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
      setStatus("Uploading...");
      const data = new FormData();
      data.append("file", file);

      const res = await axios.post("http://localhost:5240/upload", data, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      console.log("Upload response:", res.data);
      setStatus("Upload successful!");
    } catch (err) {
      console.error(err);
      setStatus("Upload failed.");
    }
  };

  return (
    <div style={{ maxWidth: 600, margin: "120px auto", textAlign: "center" }}>
      <h1>Upload Resume</h1>

      <div
        onDrop={handleDrop}
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

      <button style={{ ...buttonStyle, marginTop: 20 }} onClick={onUpload}>
        Upload & Analyze
      </button>

      {status && <p style={{ marginTop: 10 }}>{status}</p>}
    </div>
  );
}

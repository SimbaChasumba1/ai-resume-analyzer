"use client";
import React, { useRef, useState } from "react";
import axios from "axios";

const BACKEND_URL = process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240/upload";

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [dragging, setDragging] = useState(false);
  const [status, setStatus] = useState("");
  const inputRef = useRef<HTMLInputElement | null>(null);

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragging(false);
    const f = e.dataTransfer.files?.[0];
    if (f && f.type === "application/pdf") setFile(f);
    else if (f) alert("Please upload a PDF file");
  };

  const handleSelect = (f?: File | null) => {
    if (!f) return;
    if (f.type !== "application/pdf") {
      alert("Only PDFs allowed");
      return;
    }
    setFile(f);
    setStatus(`Selected: ${f.name}`);
  };

  const upload = async () => {
    if (!file) {
      setStatus("Please select a PDF.");
      return;
    }
    setStatus("Uploading...");
    try {
      const data = new FormData();
      data.append("file", file);
      const res = await axios.post(BACKEND_URL, data, {
        headers: { "Content-Type": "multipart/form-data" },
        timeout: 15000,
      });
      console.log("backend response", res.data);
      setStatus("Upload successful — analysis queued");
    } catch (err) {
      console.error(err);
      setStatus("Upload failed. Is backend running?");
    }
  };

  return (
    <div className="w-full h-screen flex items-center justify-center bg-midnightblue">
      <div className="w-full max-w-xl p-8 rounded-3xl bg-midnightblue border border-white/10 shadow-xl flex flex-col items-center gap-6">
        {/* Header */}
        <div className="flex flex-col items-center gap-3">
          <div className="w-16 h-16 flex items-center justify-center bg-gradient-to-br from-indigo-500 to-sky-400 rounded-lg shadow-md">
            <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M3 15a4 4 0 004 4h10a4 4 0 004-4v-6a4 4 0 00-4-4H7a4 4 0 00-4 4v6z" />
            </svg>
          </div>
          <h2 className="text-2xl font-bold text-white text-center">Upload Your Resume</h2>
          <p className="text-slate-300 text-center text-sm">PDF only • Recruiter-ready analysis</p>
        </div>

        {/* Upload Box */}
        <div
          onDrop={handleDrop}
          onDragOver={(e) => { e.preventDefault(); setDragging(true); }}
          onDragLeave={() => setDragging(false)}
          onClick={() => inputRef.current?.click()}
          className={`w-full mt-4 p-6 rounded-2xl border-2 border-dashed cursor-pointer flex flex-col items-center justify-center gap-3 transition-all ${
            dragging ? "border-indigo-400 bg-indigo-900/30" : "border-white/20 bg-midnightblue"
          }`}
        >
          <input type="file" ref={inputRef} className="hidden" accept="application/pdf" onChange={(e) => handleSelect(e.target.files?.[0] || null)} />
          <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 text-indigo-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4" />
          </svg>
          <div className="text-white font-medium text-center">{file ? file.name : "Click or drag a PDF here"}</div>
          <div className="text-slate-400 text-sm text-center">Professional recruiter-friendly design.</div>
          {status && <div className="text-slate-300 text-sm mt-2 text-center">{status}</div>}
        </div>

        {/* Buttons */}
        <div className="mt-4 w-full flex flex-col sm:flex-row items-center gap-3">
          <button
            onClick={upload}
            className="flex-1 px-6 py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-xl transition-all text-center"
          >
            Upload & Analyze
          </button>
          <button
            onClick={() => { setFile(null); setStatus(""); }}
            className="flex-1 px-6 py-3 border border-white/20 text-white rounded-xl hover:bg-white/10 transition-all text-center"
          >
            Clear
          </button>
        </div>
      </div>
    </div>
  );
}


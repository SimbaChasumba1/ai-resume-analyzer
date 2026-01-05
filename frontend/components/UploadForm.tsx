"use client";

import React, { useRef, useState } from "react";
import axios from "axios";
import { useRouter } from "next/router";  

const BACKEND_URL =
  process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

export default function UploadForm() {
  const [file, setFile] = useState<File | null>(null);
  const [dragging, setDragging] = useState(false);
  const [status, setStatus] = useState("");
  const inputRef = useRef<HTMLInputElement | null>(null);
  const router = useRouter();  // Initialize the router

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragging(false);
    const f = e.dataTransfer.files?.[0];
    if (f && f.type === "application/pdf") setFile(f);
    else alert("Please upload a PDF");
  };

  const upload = async () => {
    if (!file) {
      setStatus("Please select a PDF.");
      return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
      setStatus("You must be logged in to upload");
      return;
    }

    try {
      setStatus("Uploading...");
      const data = new FormData();
      data.append("file", file);

      const res = await axios.post(`${BACKEND_URL}/api/resume/upload`, data, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "multipart/form-data",
        },
      });

      // Redirect to analysis page after successful upload
      router.push(`/analysis/${res.data.analysisId}`);  // Assuming `analysisId` is returned

      setStatus("Upload successful — analysis queued");
      setFile(null);
    } catch (err: any) {
      console.error(err);
      setStatus(
        err.response?.data?.message || "Upload failed. Is Backend running?"
      );
    }
  };

  return (
    <div className="w-full min-h-screen flex items-center justify-center bg-midnightblue px-4">
      <div className="w-full max-w-xl p-8 rounded-3xl bg-white/5 border border-white/10 backdrop-blur-xl shadow-2xl flex flex-col gap-6 items-center">
        {/* Upload Icon */}
        <div className="w-16 h-16 bg-indigo-600 rounded-xl flex items-center justify-center shadow-lg">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="w-8 h-8 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={1.5}
              d="M3 15a4 4 0 004 4h10a4 4 0 004-4v-6a4 4 0 00-4-4H7a4 4 0 00-4 4v6z"
            />
          </svg>
        </div>

        <h2 className="text-2xl font-bold text-white">Upload Your Resume</h2>
        <p className="text-slate-300 text-center">
          PDF only — recruiter-ready AI analysis.
        </p>

        {/* Drag & Drop / Click Box */}
        <div
          onClick={() => inputRef.current?.click()}
          onDrop={handleDrop}
          onDragOver={(e) => {
            e.preventDefault();
            setDragging(true);
          }}
          onDragLeave={() => setDragging(false)}
          className={`w-full p-8 rounded-2xl border-2 border-dashed cursor-pointer transition flex flex-col items-center justify-center ${
            dragging
              ? "border-indigo-400 bg-indigo-900/20"
              : "border-white/20 bg-white/5"
          }`}
        >
          <input
            ref={inputRef}
            type="file"
            className="hidden"
            accept="application/pdf"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
          />

          <div className="flex flex-col items-center gap-3">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="w-12 h-12 text-indigo-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 4v16m8-8H4"
              />
            </svg>

            <span className="text-white font-medium text-center">
              {file ? file.name : "Click or drag a PDF here"}
            </span>
          </div>
        </div>

        {/* Buttons */}
        <div className="w-full flex gap-3">
          <button
            onClick={upload}
            className="flex-1 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold py-3 rounded-xl"
          >
            Upload & Analyze
          </button>

          <button
            onClick={() => {
              setFile(null);
              setStatus("");
            }}
            className="flex-1 border border-white/20 text-white py-3 rounded-xl hover:bg-white/10"
          >
            Clear
          </button>
        </div>

        {status && <p className="text-slate-300">{status}</p>}
      </div>
    </div>
  );
}

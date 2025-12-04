"use client";

import { useState } from "react";
import { ArrowUpTrayIcon } from "@heroicons/react/24/outline";

export default function Home() {
  const [dragActive, setDragActive] = useState(false);
  const [file, setFile] = useState<File | null>(null);

  const handleSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      setFile(e.target.files[0]);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragActive(false);
    if (e.dataTransfer.files?.[0]) {
      setFile(e.dataTransfer.files[0]);
    }
  };

  return (
    <main className="min-h-screen flex items-center justify-center px-4">
      <div className="w-full max-w-xl bg-[#111] p-8 rounded-2xl shadow-[0_0_50px_rgba(0,0,0,0.8)] border border-white/10">
        
        <h1 className="text-4xl font-bold mb-6">
          Resume <span className="text-indigo-500">Analyzer</span>
        </h1>

        <p className="mb-6 text-white/60">
          Upload your resume and instantly get professional insights + AI analysis.
        </p>

        {/* Upload Box */}
        <label
          onDragOver={(e) => { e.preventDefault(); setDragActive(true); }}
          onDragLeave={(e) => { e.preventDefault(); setDragActive(false); }}
          onDrop={handleDrop}
          className={`flex flex-col items-center justify-center h-52 border-2 border-dashed rounded-xl cursor-pointer transition all ${
            dragActive ? "border-indigo-500 bg-indigo-500/10" : "border-white/10 bg-white/5"
          }`}
        >
          <ArrowUpTrayIcon className="w-10 h-10 mb-3 text-white/70" />
          <p className="text-white/70 mb-2">
            {file ? (
              <span className="text-indigo-400 font-medium">{file.name}</span>
            ) : (
              "Drag & Drop your resume"
            )}
          </p>
          <p className="text-white/40 text-sm">or click to upload</p>

          <input
            type="file"
            className="hidden"
            accept=".pdf,.doc,.docx"
            onChange={handleSelect}
          />
        </label>

        {/* Submit */}
        <button
          disabled={!file}
          className="mt-6 w-full py-3 rounded-xl font-semibold bg-indigo-600 hover:bg-indigo-700 transition disabled:bg-white/10 disabled:text-white/30"
        >
          Analyze Resume
        </button>
      </div>
    </main>
  );
}

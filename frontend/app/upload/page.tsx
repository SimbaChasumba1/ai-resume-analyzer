"use client";

import { useState, useRef } from "react";
import axios from "axios";

const BACKEND_URL = "http://localhost:5240/upload";

export default function UploadPage() {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState("");
  const [dragging, setDragging] = useState(false);

  const inputRef = useRef<HTMLInputElement | null>(null);

  const handleSelect = (file: File | null) => {
    setFile(file);
    if (file) setStatus(`Selected: ${file.name}`);
  };

  const upload = async () => {
    if (!file) return setStatus("Please select a PDF first.");

    try {
      setStatus("Uploading…");

      const formData = new FormData();
      formData.append("file", file);

      const res = await axios.post(BACKEND_URL, formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      console.log(res.data);
      setStatus("Uploaded! Awaiting analysis…");
    } catch (err) {
      console.error(err);
      setStatus("Upload failed — backend unreachable.");
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center 
      bg-gradient-to-br from-black via-gray-900 to-zinc-900 px-6">

      <div className="w-full max-w-xl bg-white/10 backdrop-blur-xl border border-white/10 
        rounded-2xl shadow-2xl p-10 text-center">

        <h1 className="text-3xl font-bold text-white mb-6">Upload Your Resume</h1>

        <div
          onClick={() => inputRef.current?.click()}
          onDrop={(e) => {
            e.preventDefault();
            setDragging(false);
            handleSelect(e.dataTransfer.files[0] || null);
          }}
          onDragOver={(e) => {
            e.preventDefault();
            setDragging(true);
          }}
          onDragLeave={() => setDragging(false)}
          className={`border-2 border-dashed rounded-2xl p-10 cursor-pointer transition 
          ${dragging ? "border-white bg-white/10" : "border-gray-400/50"}`}
        >
          <input
            type="file"
            ref={inputRef}
            className="hidden"
            accept="application/pdf"
            onChange={(e) => handleSelect(e.target.files?.[0] || null)}
          />

          <p className="text-white text-lg font-medium">
            {file ? file.name : "Click or Drag Your PDF Here"}
          </p>
        </div>

        <button
          onClick={upload}
          className="mt-6 w-full py-4 bg-white text-black rounded-xl font-semibold text-lg
          hover:bg-gray-200 transition"
        >
          Upload & Analyze
        </button>

        {status && <p className="text-gray-300 mt-4">{status}</p>}
      </div>
    </div>
  );
}

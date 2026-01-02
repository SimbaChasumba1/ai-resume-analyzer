"use client";

import React, { useEffect, useState } from "react";
import axios from "axios";
import { Dialog } from "@headlessui/react";

interface Analysis {
  id: number;
  resultJson: string;
  createdAt: string;
}

interface ResumeUpload {
  id: number;
  fileName: string;
  filePath: string;
  createdAt: string;
  analysis?: Analysis;
}

const API_BASE = process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

export default function Dashboard() {
  const [uploads, setUploads] = useState<ResumeUpload[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedAnalysis, setSelectedAnalysis] = useState<Analysis | null>(null);

  useEffect(() => {
    const fetchHistory = async () => {
      const token = localStorage.getItem("token");
      if (!token) return;

      try {
        const res = await axios.get(`${API_BASE}/dashboard/history`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setUploads(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, []);

  if (loading)
    return <div className="text-white text-center mt-16">Loading...</div>;

  return (
    <div className="max-w-6xl mx-auto px-4 py-8">
      <h1 className="text-4xl font-bold text-white mb-8">Your Resume History</h1>

      {uploads.length === 0 ? (
        <p className="text-slate-400 text-lg">
          You haven’t uploaded any resumes yet. Go to <span className="font-semibold text-indigo-400">Upload</span> to start.
        </p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {uploads.map((upload) => (
            <div
              key={upload.id}
              className="bg-white/5 backdrop-blur border border-white/10 rounded-xl p-6 shadow-lg flex flex-col justify-between hover:scale-105 transition-transform"
            >
              <div>
                <div className="flex items-center justify-between mb-2">
                  <h2 className="text-xl font-semibold text-white">{upload.fileName}</h2>
                  <span
                    className={`px-2 py-1 text-sm rounded-full font-medium ${
                      upload.analysis ? "bg-green-600 text-white" : "bg-gray-600 text-white"
                    }`}
                  >
                    {upload.analysis ? "Completed" : "Pending"}
                  </span>
                </div>
                <p className="text-slate-400 text-sm mb-3">
                  Uploaded: {new Date(upload.createdAt).toLocaleString()}
                </p>
                {upload.analysis && (
                  <p className="text-slate-300 text-sm line-clamp-3">
                    {JSON.stringify(upload.analysis.resultJson)}
                  </p>
                )}
              </div>

              <div className="flex gap-3 mt-4">
                <button
                  onClick={() => upload.analysis && setSelectedAnalysis(upload.analysis)}
                  disabled={!upload.analysis}
                  className={`flex-1 py-2 rounded-lg font-semibold text-white ${
                    upload.analysis ? "bg-indigo-600 hover:bg-indigo-500" : "bg-gray-700 cursor-not-allowed"
                  } transition`}
                >
                  View Analysis
                </button>
                <a
                  href={`${API_BASE}/uploads/${upload.filePath}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="flex-1 py-2 text-center rounded-lg border border-white/20 text-white hover:bg-white/5 transition"
                >
                  Download
                </a>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Analysis Modal */}
      <Dialog open={!!selectedAnalysis} onClose={() => setSelectedAnalysis(null)}>
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4">
          <Dialog.Panel className="bg-slate-900 rounded-xl p-6 w-full max-w-lg shadow-lg">
            <Dialog.Title className="text-2xl font-bold text-white mb-4">Analysis Result</Dialog.Title>
            <pre className="bg-black/20 p-4 rounded text-sm text-white overflow-x-auto">
              {selectedAnalysis && JSON.stringify(selectedAnalysis.resultJson, null, 2)}
            </pre>
            <button
              onClick={() => setSelectedAnalysis(null)}
              className="mt-4 px-4 py-2 bg-red-600 hover:bg-red-500 text-white rounded font-semibold"
            >
              Close
            </button>
          </Dialog.Panel>
        </div>
      </Dialog>
    </div>
  );
}

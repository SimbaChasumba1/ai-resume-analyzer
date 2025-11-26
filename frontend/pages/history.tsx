// pages/history.tsx
"use client";

import { useEffect, useState } from "react";
import axios from "axios";

interface ResumeRecord {
  id: string;
  filename: string;
  createdAt: string;
}

export default function History() {
  const [records, setRecords] = useState<ResumeRecord[]>([]);

  useEffect(() => {
    axios.get("http://localhost:5240/api/resume/history")
      .then(res => setRecords(res.data))
      .catch(() => {});
  }, []);

  return (
    <div style={{ padding: "40px" }}>
      <h1>Your Resume History</h1>
      <ul>
        {records.map(r => (
          <li key={r.id}>{r.filename} — {new Date(r.createdAt).toLocaleString()}</li>
        ))}
      </ul>
    </div>
  );
}

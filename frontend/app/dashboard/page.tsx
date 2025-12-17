"use client";

import { useEffect, useState } from "react";
import { authFetch, logout } from "@/lib/auth";
import Link from "next/link";

type ResumeItem = {
  id: string;
  createdAt: string;
  status: string;
};

export default function DashboardPage() {
  const [resumes, setResumes] = useState<ResumeItem[]>([]);

  useEffect(() => {
    authFetch("http://localhost:5240/history")
      .then((res: Response) => res.json())
      .then((data: ResumeItem[]) => setResumes(data));
  }, []);

  return (
    <div className="min-h-screen bg-theme p-8">
      <div className="max-w-5xl mx-auto space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-3xl font-bold">Dashboard</h1>
          <button onClick={logout} className="text-red-400">
            Logout
          </button>
        </div>

        <div className="grid gap-4">
          {resumes.map((r) => (
            <Link
              key={r.id}
              href={`/analysis/${r.id}`}
              className="bg-box p-4 rounded-xl border border-white/10 hover:border-indigo-500 transition"
            >
              <div className="flex justify-between">
                <span>Resume uploaded</span>
                <span className="text-slate-400">{r.status}</span>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}

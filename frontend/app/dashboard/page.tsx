"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import axios from "axios";

interface ResumeUpload {
  id: number;
  fileName: string;
  createdAt: string;
  score?: number;
}

interface DashboardData {
  user: {
    email: string;
    name?: string;
  };
  uploads: ResumeUpload[];
}

export default function DashboardPage() {
  const router = useRouter();
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      router.push("/");
      return;
    }

    axios
      .get("http://localhost:5000/api/dashboard", {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .then((res) => {
        setData(res.data);
      })
      .catch(() => {
        localStorage.removeItem("token");
        router.push("/");
      })
      .finally(() => setLoading(false));
  }, [router]);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center text-slate-400">
        Loading dashboard…
      </div>
    );
  }

  if (!data) return null;

  return (
    <div className="min-h-screen bg-[#0b0f1a] text-white px-8 py-10">
      <div className="max-w-6xl mx-auto space-y-10">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold">Dashboard</h1>
            <p className="text-slate-400 text-sm">
              Logged in as {data.user.email}
            </p>
          </div>
        </div>

        {/* Upload History */}
        <div>
          <h2 className="text-xl font-semibold mb-4">Resume History</h2>

          {data.uploads.length === 0 ? (
            <div className="rounded-xl border border-white/10 p-10 text-center text-slate-400">
              No resumes uploaded yet.
            </div>
          ) : (
            <div className="grid gap-4">
              {data.uploads.map((u) => (
                <div
                  key={u.id}
                  className="flex justify-between items-center rounded-xl border border-white/10 bg-white/5 px-6 py-4 hover:bg-white/10 transition"
                >
                  <div>
                    <p className="font-medium">{u.fileName}</p>
                    <p className="text-xs text-slate-400">
                      {new Date(u.createdAt).toLocaleString()}
                    </p>
                  </div>

                  <div className="text-right">
                    {u.score !== undefined ? (
                      <span className="text-green-400 font-semibold">
                        Score: {u.score}%
                      </span>
                    ) : (
                      <span className="text-yellow-400 text-sm">
                        Processing…
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

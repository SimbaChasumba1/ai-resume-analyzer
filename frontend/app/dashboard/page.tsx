"use client";

import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

interface Analysis {
  id: string;
  resumeFileName: string;
  atsScore: number;
  summary: string;
}

interface Resume {
  jobId: string;
  resumeFileName: string;
  status: string;
  createdAt: string;
  completedAt: string | null;
  analysis: Analysis | null;
}

interface UserProfile {
  email: string;
  createdAt?: string;
}

const BACKEND_URL =
  (process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240").replace(/\/+$/, "");

export default function DashboardPage() {
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<UserProfile | null>(null);

  const router = useRouter();

  useEffect(() => {
    const fetchDashboard = async () => {
      const token = localStorage.getItem("token");

      if (!token) {
        router.replace("/");
        return;
      }

      try {
        const res = await axios.get(`${BACKEND_URL}/api/dashboard`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        setUser(res.data.user);
        setResumes(res.data.uploads ?? []);
      } catch (err) {
        console.error("Failed to load dashboard:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, [router]);

  return (
    <div className="p-8 min-h-screen bg-midnightblue text-white">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

      {user && (
        <div className="mb-6 p-4 bg-white/10 rounded-xl">
          <h2 className="text-xl font-semibold">User Info</h2>
          <p>Email: {user.email}</p>
        </div>
      )}

      <div className="mb-6">
        <h2 className="text-xl font-semibold mb-3">Resume History</h2>

        {loading ? (
          <p>Loading resumes...</p>
        ) : resumes.length === 0 ? (
          <p>No resumes uploaded yet.</p>
        ) : (
          <ul className="space-y-3">
            {resumes.map((resume) => {
              const completed =
                resume.status?.toLowerCase() === "completed" &&
                resume.analysis !== null;

              return (
                <li
                  key={resume.jobId}
                  className="p-4 bg-white/5 rounded-lg flex justify-between items-center shadow-inner hover:bg-white/10 transition gap-4"
                >
                  <div className="min-w-0">
                    <p className="text-white font-medium truncate">
                      {resume.resumeFileName}
                    </p>

                    <p className="text-sm text-slate-400">
                      {new Date(resume.createdAt).toLocaleString()}
                    </p>

                    <p className="text-sm mt-1">
                      Status:{" "}
                      <span
                        className={
                          completed ? "text-green-400" : "text-yellow-400"
                        }
                      >
                        {resume.status}
                      </span>
                    </p>
                  </div>

                  <div className="flex items-center gap-3 shrink-0">
                    {completed ? (
                      <button
                        onClick={() =>
                          router.push(`/analysis/${resume.jobId}`)
                        }
                        className="px-3 py-1 bg-indigo-600 hover:bg-indigo-500 rounded-md text-white text-sm"
                      >
                        View
                      </button>
                    ) : (
                      <button
                        onClick={() =>
                          router.push(`/analysis/${resume.jobId}`)
                        }
                        className="px-3 py-1 bg-white/10 hover:bg-white/20 rounded-md text-white text-sm"
                      >
                        View Status
                      </button>
                    )}
                  </div>
                </li>
              );
            })}
          </ul>
        )}
      </div>
    </div>
  );
}
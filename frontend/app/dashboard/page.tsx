"use client";

import { useEffect, useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

interface Resume {
  id: string;
  resumeFileName: string; // <-- fix field name
  createdAt: string;
}

interface UserProfile {
  email: string;
  name?: string;
}

export default function DashboardPage() {
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [loadingResumes, setLoadingResumes] = useState(true);
  const [loadingUser, setLoadingUser] = useState(true);
  const [user, setUser] = useState<UserProfile | null>(null);

  const router = useRouter();
  const BACKEND_URL =
    process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) return;

    const fetchUser = async () => {
      try {
        const res = await axios.get(`${BACKEND_URL}/api/user/me`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setUser(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoadingUser(false);
      }
    };

    const fetchResumes = async () => {
      try {
        const res = await axios.get(`${BACKEND_URL}/api/resume/my`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setResumes(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoadingResumes(false);
      }
    };

    fetchUser();
    fetchResumes();
  }, []);

  const deleteResume = async (id: string) => {
    const token = localStorage.getItem("token");
    if (!token) return;

    try {
      await axios.delete(`${BACKEND_URL}/api/resume/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setResumes(resumes.filter((r) => r.id !== id));
    } catch (err) {
      console.error("Failed to delete resume:", err);
    }
  };

  return (
    <div className="p-8 min-h-screen bg-midnightblue text-white">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

      {loadingUser ? (
        <p>Loading user info...</p>
      ) : user ? (
        <div className="mb-6 p-4 bg-white/10 rounded-xl">
          <h2 className="text-xl font-semibold">User Info</h2>
          <p>Email: {user.email}</p>
          {user.name && <p>Name: {user.name}</p>}
        </div>
      ) : null}

      <div className="mb-6">
        <h2 className="text-xl font-semibold mb-3">Resume History</h2>
        {loadingResumes ? (
          <p>Loading resumes...</p>
        ) : resumes.length === 0 ? (
          <p>No resumes uploaded yet.</p>
        ) : (
          <ul className="space-y-3">
            {resumes.map((r) => (
              <li
                key={r.id}
                className="p-4 bg-white/5 rounded-lg flex justify-between items-center shadow-inner hover:bg-white/10 transition"
              >
                {/* File name on the far left */}
                <span className="text-white font-medium truncate max-w-xs">
                  {r.resumeFileName}
                </span>

                {/* Buttons and date */}
                <div className="flex items-center gap-3">
                  <span className="text-sm text-slate-300">
                    {new Date(r.createdAt).toLocaleString()}
                  </span>
                  <button
                    onClick={() => router.push(`/analysis/${r.id}`)}
                    className="px-3 py-1 bg-indigo-600 hover:bg-indigo-500 rounded-md text-white text-sm"
                  >
                    View
                  </button>
                  <button
                    onClick={() => deleteResume(r.id)}
                    className="px-3 py-1 bg-red-600 hover:bg-red-500 rounded-md text-white text-sm"
                  >
                    Delete
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}

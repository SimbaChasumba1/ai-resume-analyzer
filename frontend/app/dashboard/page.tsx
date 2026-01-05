"use client";



import { useEffect, useState } from "react";

import axios from "axios";



interface Resume {

  id: string;

  fileName: string;

  createdAt: string;

}



interface UserProfile {

  email: string;

  name?: string;

}



export default function DashboardPage() {

  const [resumes, setResumes] = useState<Resume[]>([]);

  const [loading, setLoading] = useState(true);

  const [user, setUser] = useState<UserProfile | null>(null);



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

        setLoading(false);

      }

    };



    fetchUser();

    fetchResumes();

  }, []);



  return (

    <div className="p-8 min-h-screen bg-midnightblue text-white">

      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>



      {user && (

        <div className="mb-6 p-4 bg-white/10 rounded-xl">

          <h2 className="text-xl font-semibold">User Info</h2>

          <p>Email: {user.email}</p>

          {user.name && <p>Name: {user.name}</p>}

        </div>

      )}



      <div className="mb-6">

        <h2 className="text-xl font-semibold mb-3">Resume History</h2>

        {loading ? (

          <p>Loading...</p>

        ) : resumes.length === 0 ? (

          <p>No resumes uploaded yet.</p>

        ) : (

          <ul className="space-y-2">

            {resumes.map((r) => (

              <li

                key={r.id}

                className="p-3 bg-white/5 rounded-lg flex justify-between items-center"

              >

                <span>{r.fileName}</span>

                <span className="text-sm text-slate-300">

                  {new Date(r.createdAt).toLocaleString()}

                </span>

              </li>

            ))}

          </ul>

        )}

      </div>

    </div>

  );

}


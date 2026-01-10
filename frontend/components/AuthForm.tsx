"use client";

import React, { useState } from "react";
import axios from "axios";
import { useRouter } from "next/navigation";

const API_BASE = process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

type Mode = "login" | "signup";

interface AuthFormProps {
  initialMode?: Mode;
  onClose?: () => void;
}

export default function AuthForm({ initialMode = "login", onClose }: AuthFormProps) {
  const router = useRouter();
  const [mode, setMode] = useState<Mode>(initialMode);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [status, setStatus] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const submit = async () => {
    if (!email || !password) {
      setStatus("Email and password required");
      return;
    }

    setLoading(true);
    try {
      const endpoint = mode === "signup" ? "/auth/signup" : "/auth/login";
      const res = await axios.post(`${API_BASE}${endpoint}`, { email, password });
      const token = res.data?.token;
      if (!token) throw new Error("No token returned");

      localStorage.setItem("token", token);
      onClose?.();
      router.push("/upload");
      router.refresh();
    } catch (err: any) {
      console.error(err);
      if (axios.isAxiosError(err)) setStatus(err.response?.data ?? "Authentication failed");
      else setStatus("Authentication failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 px-4">
      <div className="relative w-full max-w-md bg-white/5 backdrop-blur border border-white/10 rounded-2xl p-8 space-y-6">
        
        {/* ✕ Close button */}
        {onClose && (
          <button
            onClick={onClose}
            aria-label="Close"
            className="absolute top-4 right-4 text-slate-400 hover:text-white text-xl leading-none"
          >
            ×
          </button>
        )}

        <h1 className="text-2xl font-bold text-white text-center">
          {mode === "login" ? "Login" : "Create Account"}
        </h1>

        <input
          type="email"
          placeholder="Email"
          className="w-full px-4 py-3 rounded-lg bg-black/40 text-white border border-white/20 outline-none"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Password"
          className="w-full px-4 py-3 rounded-lg bg-black/40 text-white border border-white/20 outline-none"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button
          onClick={submit}
          disabled={loading}
          className="w-full py-3 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-white font-semibold transition disabled:opacity-50"
        >
          {loading ? "Please wait..." : mode === "login" ? "Login" : "Sign Up"}
        </button>

        <div className="text-center text-slate-400 text-sm">
          {mode === "login" ? (
            <>
              Don’t have an account?{" "}
              <button onClick={() => setMode("signup")} className="text-indigo-400 hover:underline">
                Sign up
              </button>
            </>
          ) : (
            <>
              Already have an account?{" "}
              <button onClick={() => setMode("login")} className="text-indigo-400 hover:underline">
                Login
              </button>
            </>
          )}
        </div>

        {status && <div className="text-center text-sm text-slate-300">{status}</div>}
      </div>
    </div>
  );
}

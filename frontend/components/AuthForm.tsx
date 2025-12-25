"use client";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { setAuthToken } from "@/lib/auth";

const API_BASE = process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

export default function AuthForm({
  initialMode = "login",
  onClose,
}: {
  initialMode?: "login" | "signup";
  onClose: () => void;
}) {
  const [mode, setMode] = useState<"login" | "signup">(initialMode);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const router = useRouter();

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const endpoint =
        mode === "login" ? "/auth/login" : "/auth/register";

      const res = await fetch(`${API_BASE}${endpoint}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || "Authentication failed");
      }

      const data: { token: string } = await res.json();

      setAuthToken(data.token);

      onClose();
      router.push("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Auth failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/60" onClick={onClose} />
      <div className="relative w-full max-w-md p-6 bg-slate-900 rounded-2xl">
        <h3 className="text-xl font-semibold mb-4 text-white">
          {mode === "login" ? "Log in" : "Create account"}
        </h3>

        {error && (
          <div className="mb-3 text-sm text-red-400 bg-red-500/10 p-2 rounded">
            {error}
          </div>
        )}

        <form onSubmit={submit} className="space-y-4">
          <input
            required
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full p-3 rounded bg-slate-800 text-white"
          />

          <input
            required
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full p-3 rounded bg-slate-800 text-white"
          />

          <button
            type="submit"
            disabled={loading}
            className="w-full py-3 bg-indigo-600 rounded font-semibold text-white disabled:opacity-50"
          >
            {loading
              ? "Processing..."
              : mode === "login"
              ? "Log in"
              : "Create account"}
          </button>

          <p className="text-sm text-center text-slate-400">
            {mode === "login" ? (
              <>
                Don’t have an account?{" "}
                <button
                  type="button"
                  className="text-indigo-400"
                  onClick={() => setMode("signup")}
                >
                  Sign up
                </button>
              </>
            ) : (
              <>
                Already have an account?{" "}
                <button
                  type="button"
                  className="text-indigo-400"
                  onClick={() => setMode("login")}
                >
                  Log in
                </button>
              </>
            )}
          </p>
        </form>
      </div>
    </div>
  );
}

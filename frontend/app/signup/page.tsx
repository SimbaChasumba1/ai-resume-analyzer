"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { setAuthToken } from "@/lib/auth";

export default function SignupPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSignup(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240"}/auth/signup`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, password }),
        }
      );

      if (!res.ok) {
        const msg = await res.text();
        throw new Error(msg || "Signup failed");
      }

      const data: { token: string } = await res.json();
      setAuthToken(data.token);

      router.push("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Signup failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-theme">
      <form
        onSubmit={handleSignup}
        className="w-full max-w-md bg-box p-8 rounded-2xl shadow-xl space-y-6"
      >
        <h1 className="text-3xl font-bold text-center">Create account</h1>

        {error && (
          <div className="bg-red-500/10 text-red-400 text-sm p-3 rounded-lg">
            {error}
          </div>
        )}

        <input
          type="email"
          placeholder="Email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full px-4 py-3 rounded-lg bg-black/20 border border-white/10"
        />

        <input
          type="password"
          placeholder="Password"
          required
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full px-4 py-3 rounded-lg bg-black/20 border border-white/10"
        />

        <button
          type="submit"
          disabled={loading}
          className="w-full py-3 rounded-lg bg-indigo-600 hover:bg-indigo-500 font-semibold disabled:opacity-50"
        >
          {loading ? "Creating account..." : "Sign up"}
        </button>

        <p className="text-sm text-center text-muted">
          Already have an account?{" "}
          <a href="/login" className="text-indigo-400 hover:underline">
            Log in
          </a>
        </p>
      </form>
    </div>
  );
}

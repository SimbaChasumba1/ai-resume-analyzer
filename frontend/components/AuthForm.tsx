"use client";
import React, { useState } from "react";

export default function AuthForm({ initialMode = "login", onClose }: { initialMode?: "login" | "signup"; onClose: () => void; }) {
  const [mode, setMode] = useState<"login" | "signup">(initialMode);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    // placeholder: call your auth API (NextAuth / custom)
    console.log("auth submit", { mode, name, email, password });
    // TODO: replace with real API call
    alert(`${mode === "login" ? "Logged in" : "Signed up"} (demo)`);
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/60" onClick={onClose} />
      <div className="relative w-full max-w-md p-6 bg-slate-900/90 border border-white/6 rounded-2xl shadow-xl">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold text-white">{mode === "login" ? "Log in" : "Create account"}</h3>
          <button className="text-slate-400" onClick={onClose} aria-label="Close">✕</button>
        </div>

        <form onSubmit={submit} className="space-y-4">
          {mode === "signup" && (
            <div>
              <label className="text-sm text-slate-300">Full name</label>
              <input value={name} onChange={(e) => setName(e.target.value)} className="mt-1 w-full p-3 rounded-xl bg-slate-800 border border-white/6 text-white" placeholder="Jane Doe" />
            </div>
          )}

          <div>
            <label className="text-sm text-slate-300">Email</label>
            <input value={email} onChange={(e) => setEmail(e.target.value)} className="mt-1 w-full p-3 rounded-xl bg-slate-800 border border-white/6 text-white" placeholder="you@company.com" />
          </div>

          <div>
            <label className="text-sm text-slate-300">Password</label>
            <input value={password} onChange={(e) => setPassword(e.target.value)} type="password" className="mt-1 w-full p-3 rounded-xl bg-slate-800 border border-white/6 text-white" placeholder="••••••••" />
          </div>

          <button type="submit" className="w-full py-3 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-semibold">
            {mode === "login" ? "Log in" : "Create account"}
          </button>

          <div className="text-center text-sm text-slate-400">
            {mode === "login" ? (
              <>Don’t have an account? <button className="text-indigo-400" onClick={() => setMode("signup")}>Sign up</button></>
            ) : (
              <>Already have an account? <button className="text-indigo-400" onClick={() => setMode("login")}>Log in</button></>
            )}
          </div>
        </form>
      </div>
    </div>
  );
}

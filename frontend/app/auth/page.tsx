"use client";

import { useState } from "react";
import AuthForm from "@/components/AuthForm";

export default function AuthPage() {
  const [mode, setMode] = useState<"login" | "signup">("login");

  return (
    <div className="min-h-screen bg-gradient-to-br from-black via-gray-900 to-zinc-900 flex flex-col items-center">
      
      <div className="w-full max-w-md mt-32 bg-white/10 backdrop-blur-xl rounded-2xl p-10 shadow-xl border border-white/10">
        <div className="flex mb-6">
          <button
            onClick={() => setMode("login")}
            className={`flex-1 py-3 text-lg font-semibold rounded-xl transition 
              ${mode === "login" ? "bg-white text-black" : "text-white hover:bg-white/10"}`}
          >
            Login
          </button>
          <button
            onClick={() => setMode("signup")}
            className={`flex-1 py-3 text-lg font-semibold rounded-xl transition 
              ${mode === "signup" ? "bg-white text-black" : "text-white hover:bg-white/10"}`}
          >
            Sign Up
          </button>
        </div>

        <AuthForm mode={mode} />
      </div>
    </div>
  );
}

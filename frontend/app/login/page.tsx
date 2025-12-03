"use client";

import { useState } from "react";
import AuthForm from "@/components/AuthForm";

export default function AuthPage() {
  const [mode, setMode] = useState<"login" | "signup">("login");

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-md bg-white shadow-xl rounded-2xl p-8 border border-gray-100">
        <h1 className="text-3xl font-bold text-center mb-6">
          {mode === "login" ? "Welcome Back" : "Create Your Account"}
        </h1>

        {/* Toggle */}
        <div className="flex justify-center space-x-4 mb-6">
          <button
            className={`px-4 py-2 rounded-lg font-medium ${
              mode === "login" ? "bg-black text-white" : "bg-gray-200"
            }`}
            onClick={() => setMode("login")}
          >
            Login
          </button>
          <button
            className={`px-4 py-2 rounded-lg font-medium ${
              mode === "signup" ? "bg-black text-white" : "bg-gray-200"
            }`}
            onClick={() => setMode("signup")}
          >
            Sign Up
          </button>
        </div>

        <AuthForm mode={mode} />
      </div>
    </div>
  );
}

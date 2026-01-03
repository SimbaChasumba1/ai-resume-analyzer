"use client";

import Link from "next/link";
import React, { useEffect, useState } from "react";
import AuthForm from "./AuthForm";
import { isLoggedIn, logout } from "@/lib/auth";

export default function Navbar() {
  const [authOpen, setAuthOpen] = useState(false);
  const [mode, setMode] = useState<"login" | "signup">("login");
  const [loggedIn, setLoggedIn] = useState(false);

  useEffect(() => {
    setLoggedIn(isLoggedIn());
  }, []);

  return (
    <>
      <nav className="w-full border-b border-white/10 bg-transparent backdrop-blur-sm">
        <div className="max-w-6xl mx-auto px-6 py-4 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg flex items-center justify-center bg-gradient-to-br from-indigo-500 to-sky-400 shadow-lg">
              <span className="font-extrabold text-white text-lg">AI</span>
            </div>
            <div>
              <div className="font-semibold text-white text-lg">
                AIResumeAnalyzer
              </div>
              <div className="text-xs text-slate-400 -mt-0.5">
                Resume analysis for hiring-ready portfolios
              </div>
            </div>
          </Link>

          <div className="flex items-center gap-4">
            {loggedIn ? (
              <>
                <Link
                  href="/upload"
                  className="rounded-md px-4 py-2 text-sm bg-white/5 hover:bg-white/10 transition"
                >
                  Upload
                </Link>

                <Link
                  href="/dashboard"
                  className="rounded-md px-4 py-2 text-sm bg-white/5 hover:bg-white/10 transition"
                >
                  Dashboard
                </Link>

                <button
                  onClick={logout}
                  className="px-4 py-2 rounded-md bg-red-600/90 hover:bg-red-600 text-white text-sm"
                >
                  Logout
                </button>
              </>
            ) : (
              <>
                <button
                  onClick={() => {
                    setAuthOpen(true);
                    setMode("login");
                  }}
                  className="px-4 py-2 rounded-md bg-indigo-600 hover:bg-indigo-500 text-white text-sm shadow"
                >
                  Log in
                </button>

                <button
                  onClick={() => {
                    setAuthOpen(true);
                    setMode("signup");
                  }}
                  className="px-4 py-2 rounded-md border border-white/10 text-sm hover:bg-white/5 transition"
                >
                  Sign up
                </button>
              </>
            )}
          </div>
        </div>
      </nav>

      {authOpen && (
        <AuthForm
          initialMode={mode}
          onClose={() => {
            setAuthOpen(false);
            setLoggedIn(isLoggedIn()); //  refresh navbar after auth
          }}
        />
      )}
    </>
  );
}

"use client";

import { useState } from "react";

export default function AuthForm({ mode }: { mode: "login" | "signup" }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  return (
    <form className="flex flex-col gap-5">
      <input
        type="email"
        placeholder="Email"
        className="w-full p-4 rounded-xl bg-white/20 text-white placeholder-gray-300 
         border border-white/20 focus:bg-white/30 outline-none transition"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />

      <input
        type="password"
        placeholder="Password"
        className="w-full p-4 rounded-xl bg-white/20 text-white placeholder-gray-300 
         border border-white/20 focus:bg-white/30 outline-none transition"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <button
        type="submit"
        className="w-full py-4 bg-white text-black rounded-xl text-lg font-bold shadow 
        hover:bg-gray-200 transition"
      >
        {mode === "login" ? "Login" : "Create Account"}
      </button>
    </form>
  );
}


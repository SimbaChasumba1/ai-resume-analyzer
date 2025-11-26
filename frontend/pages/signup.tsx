"use client";
import { useState } from "react";
import api from "../utils/api";

export default function Signup() {
  const [email, setEmail] = useState("");
  const [pass, setPass] = useState("");
  const [msg, setMsg] = useState("");

  const signup = async () => {
    try {
      const res = await api.post("/auth/register", {
        email,
        password: pass,
      });
      localStorage.setItem("token", res.data.token);
      window.location.href = "/";
    } catch {
      setMsg("Signup failed");
    }
  };

  return (
    <div className="max-w-md mx-auto mt-20 p-6 bg-white shadow rounded-xl">
      <h1 className="text-xl font-bold mb-4">Create Account</h1>

      <input
        className="w-full p-3 border rounded mb-3"
        placeholder="Email"
        onChange={(e) => setEmail(e.target.value)}
      />
      <input
        type="password"
        className="w-full p-3 border rounded mb-3"
        placeholder="Password"
        onChange={(e) => setPass(e.target.value)}
      />

      <button
        className="w-full bg-black text-white py-3 rounded"
        onClick={signup}
      >
        Signup
      </button>

      <p className="text-center mt-3">{msg}</p>
    </div>
  );
}

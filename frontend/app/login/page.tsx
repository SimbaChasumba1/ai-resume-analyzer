"use client";

import { useRouter } from "next/navigation";
import { setAuthToken } from "@/lib/auth";
import { useState } from "react";

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const login = async (e: React.FormEvent) => {
    e.preventDefault();

    const res = await fetch("http://localhost:5240/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    const data = await res.json();
    setAuthToken(data.token);
    router.push("/dashboard");
  };

  return (
    <form onSubmit={login} className="auth-card">
      <h1>Login</h1>
      <input value={email} onChange={e => setEmail(e.target.value)} />
      <input type="password" value={password} onChange={e => setPassword(e.target.value)} />
      <button>Login</button>
    </form>
  );
}

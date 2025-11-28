"use client";
import { useState } from "react";
import { buttonStyle } from "@/components/styles/buttonStyle";


export default function LoginPage() {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState("");
  const [pw, setPw] = useState("");
  const [status, setStatus] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setStatus(isLogin ? "Logging in..." : "Signing up...");
    await new Promise((r) => setTimeout(r, 800));
    setStatus(isLogin ? "Logged in (mock)" : "Account created (mock)");
  };

  return (
    <main style={{ paddingTop: 120, display: "flex", justifyContent: "center" }}>
      <div style={{ width: 380, background: "white", padding: 30, borderRadius: 12 }}>
        <h2>{isLogin ? "Login" : "Sign up"}</h2>

        <form onSubmit={submit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
          <input value={pw} onChange={(e) => setPw(e.target.value)} placeholder="Password" type="password" />

          <button type="submit" style={buttonStyle}>
            {isLogin ? "Login" : "Sign up"}
          </button>
        </form>

        <p style={{ marginTop: 12 }}>
          {isLogin ? "No account?" : "Have an account?"}
          <button onClick={() => setIsLogin(!isLogin)} style={{ marginLeft: 8 }}>
            {isLogin ? "Sign up" : "Login"}
          </button>
        </p>

        {status && <p>{status}</p>}
      </div>
    </main>
  );
}

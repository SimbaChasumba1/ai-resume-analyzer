"use client";

import { useState } from "react";
import Navbar from "../components/Navbar";

export default function LoginPage() {
  const [isLogin, setIsLogin] = useState(true); // toggle login/signup
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [status, setStatus] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!email || !password) {
      setStatus("Please fill in all fields.");
      return;
    }

    try {
      setStatus(isLogin ? "Logging in..." : "Signing up...");
      // TODO: Add API call for login/signup
      await new Promise((res) => setTimeout(res, 1000)); // fake delay
      setStatus(isLogin ? "Logged in successfully!" : "Account created!");
    } catch {
      setStatus("Something went wrong. Try again.");
    }
  };

  return (
    <>
      <Navbar />
      <div style={styles.container}>
        <div style={styles.card}>
          <h1 style={styles.title}>{isLogin ? "Login" : "Sign Up"}</h1>

          <form onSubmit={handleSubmit} style={styles.form}>
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              style={styles.input}
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              style={styles.input}
            />
            <button type="submit" style={styles.button}>
              {isLogin ? "Login" : "Sign Up"}
            </button>
          </form>

          {status && <p style={styles.status}>{status}</p>}

          <p style={styles.toggleText}>
            {isLogin ? "Don't have an account?" : "Already have an account?"}{" "}
            <span
              style={styles.toggleLink}
              onClick={() => {
                setIsLogin(!isLogin);
                setStatus("");
              }}
            >
              {isLogin ? "Sign Up" : "Login"}
            </span>
          </p>
        </div>
      </div>
    </>
  );
}

const styles: Record<string, any> = {
  container: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "100vh",
    background: "#f4f4f4",
    paddingTop: 80, // so navbar doesn't overlap
  },
  card: {
    background: "white",
    padding: "40px 30px",
    borderRadius: 12,
    boxShadow: "0 4px 20px rgba(0,0,0,0.1)",
    width: 360,
    textAlign: "center",
  },
  title: { fontSize: 24, fontWeight: 700, marginBottom: 20 },
  form: { display: "flex", flexDirection: "column", gap: 15 },
  input: {
    padding: "12px 15px",
    borderRadius: 8,
    border: "1px solid #ccc",
    fontSize: 16,
  },
  button: {
    padding: "12px 0",
    borderRadius: 8,
    background: "black",
    color: "white",
    fontWeight: 600,
    border: "none",
    cursor: "pointer",
    marginTop: 10,
  },
  status: { marginTop: 10, color: "#333", fontSize: 14 },
  toggleText: { marginTop: 15, fontSize: 14, color: "#555" },
  toggleLink: {
    color: "#2563eb",
    cursor: "pointer",
    fontWeight: 600,
  },
};

"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

export default function SignupPage() {
  const router = useRouter();
  const [fullName, setFullName] = useState("");
  const [email, setEmail]     = useState("");
  const [password, setPassword] = useState("");

  const handleSignup = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const res = await fetch("http://localhost:5240/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: fullName,
          email,
          password
        }),
      });

      if (!res.ok) {
        alert("Registration failed");
        return;
      }

      alert("Account created! Please login.");
      router.push("/login");
    } catch (error) {
      alert("Signup failed — backend not running?");
      console.error(error);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center mt-24 px-4">
      <h1 className="text-3xl font-semibold mb-6">Sign Up</h1>

      <form
        onSubmit={handleSignup}
        className="bg-[#141417] p-8 rounded-xl border border-[#2a2a2d] w-full max-w-md"
      >
        <input
          type="text"
          placeholder="Full Name"
          value={fullName}
          onChange={e => setFullName(e.target.value)}
          className="w-full p-3 mb-4 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={e => setEmail(e.target.value)}
          className="w-full p-3 mb-4 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          className="w-full p-3 mb-6 rounded-lg bg-[#0F0F11] border border-[#2a2a2d]"
        />

        <button
          type="submit"
          className="w-full py-3 rounded-lg bg-[#6A5ACD] hover:bg-[#7d6dff]"
        >
          Create Account
        </button>

        <p className="text-center text-sm mt-4">
          Already registered?{" "}
          <span
            className="text-[#6A5ACD] cursor-pointer"
            onClick={() => router.push("/login")}
          >
            Login
          </span>
        </p>
      </form>
    </div>
  );
}

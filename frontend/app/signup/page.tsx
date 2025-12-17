"use client";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { API } from "@/lib/auth";


export default function SignupPage() {
const router = useRouter();
const [email, setEmail] = useState("");
const [password, setPassword] = useState("");
const [name, setName] = useState("");


const submit = async (e: React.FormEvent) => {
e.preventDefault();
await fetch(`${API}/auth/register`, {
method: "POST",
headers: { "Content-Type": "application/json" },
body: JSON.stringify({ name, email, password })
});


router.push("/login");
};


return (
<div className="min-h-screen flex items-center justify-center">
<form onSubmit={submit} className="w-full max-w-md bg-white/5 p-8 rounded-2xl border border-white/10">
<h1 className="text-2xl font-bold mb-6">Create account</h1>
<input value={name} onChange={e => setName(e.target.value)} placeholder="Full name" className="input" />
<input value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" className="input" />
<input value={password} onChange={e => setPassword(e.target.value)} type="password" placeholder="Password" className="input" />
<button className="btn-primary w-full mt-4">Sign up</button>
</form>
</div>
);
}
"use client";
import { useEffect, useState } from "react";
import { authFetch } from "@/lib/auth";


export default function Dashboard() {
const [count, setCount] = useState(0);


useEffect(() => {
authFetch("http://localhost:5240/history")
.then(res => res.json())
.then(data => setCount(data.length));
}, []);


return (
<div className="p-8">
<h1 className="text-2xl font-bold">Dashboard</h1>
<p className="mt-4">Resumes analyzed: {count}</p>
</div>
);
}
"use client";
import { useEffect, useState } from "react";
import { authFetch } from "@/lib/auth";


export default function History() {
const [items, setItems] = useState<any[]>([]);


useEffect(() => {
authFetch("http://localhost:5240/history")
.then(res => res.json())
.then(setItems);
}, []);


return (
<div className="p-8">
<h1 className="text-xl font-semibold">Resume History</h1>
<ul className="mt-6 space-y-4">
{items.map(i => (
<li key={i.id} className="p-4 border rounded">
<p>{i.fileName}</p>
<p className="text-sm opacity-70">{new Date(i.createdAt).toLocaleString()}</p>
</li>
))}
</ul>
</div>
);
}
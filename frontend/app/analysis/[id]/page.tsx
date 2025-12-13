"use client";
import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { authFetch } from "@/lib/auth";


export default function AnalysisPage() {
const { id } = useParams();
const [data, setData] = useState<any>(null);


useEffect(() => {
authFetch(`http://localhost:5240/analysis/${id}`)
.then(res => res.json())
.then(setData);
}, [id]);


if (!data) return null;


return (
<div className="p-8 space-y-6">
<h1 className="text-2xl font-bold">Analysis</h1>
<section><h3>Summary</h3><p>{data.summary}</p></section>
<section><h3>Strengths</h3><p>{data.strengths}</p></section>
<section><h3>Weaknesses</h3><p>{data.weaknesses}</p></section>
<section><h3>Suggestions</h3><p>{data.suggestions}</p></section>
</div>
);
}
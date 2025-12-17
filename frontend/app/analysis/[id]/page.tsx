"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { authFetch } from "@/lib/auth";

type AnalysisResponse = {
  summary: string;
  skills: string;
  weaknesses: string;
  recommendations: string;
  score?: number;
};

export default function AnalysisPage() {
  const { id } = useParams<{ id: string }>();
  const [data, setData] = useState<AnalysisResponse | null>(null);

  useEffect(() => {
    authFetch(`http://localhost:5240/analysis/${id}`)
      .then((res: Response) => res.json())
      .then((json: AnalysisResponse) => setData(json));
  }, [id]);

  if (!data) return <div className="p-8 text-slate-400">Loading analysis…</div>;

  return (
    <div className="min-h-screen bg-theme px-6 py-12">
      <div className="max-w-4xl mx-auto space-y-8">
        <h1 className="text-3xl font-bold">Resume Analysis</h1>

        <Card title="Summary">{data.summary}</Card>
        <Card title="Skills">{data.skills}</Card>
        <Card title="Weaknesses">{data.weaknesses}</Card>
        <Card title="Recommendations">{data.recommendations}</Card>
      </div>
    </div>
  );
}

function Card({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <div className="bg-box p-6 rounded-2xl border border-white/10">
      <h3 className="text-xl font-semibold mb-2">{title}</h3>
      <p className="text-slate-300 leading-relaxed">{children}</p>
    </div>
  );
}

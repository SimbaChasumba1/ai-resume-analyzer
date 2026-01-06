"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import axios from "axios";

import AtsRadial from "@/components/AtsRadial";
import ResumePreview from "@/components/ResumePreview";
import { exportAnalysis } from "@/utils/exportAnalysis";

type _Debug = Parameters<typeof AtsRadial>;

const API_BASE =
  process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240";

interface AnalysisResult {
  id: string;
  resumeFileName: string;
  summary: string;
  atsScore: number;
  strengths: string[];
  weaknesses: string[];
  improvements: string[];
  createdAt: string;
}

export default function AnalysisPage() {
  const { id } = useParams();
  const router = useRouter();

  const [analysis, setAnalysis] = useState<AnalysisResult | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchAnalysis = async () => {
      const token = localStorage.getItem("token");
      if (!token) {
        router.push("/");
        return;
      }

      try {
        const res = await axios.get(
          `${API_BASE}/api/resume/analysis/${id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        setAnalysis(res.data);
      } catch (err) {
        console.error(err);
        router.push("/dashboard");
      } finally {
        setLoading(false);
      }
    };

    fetchAnalysis();
  }, [id, router]);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center text-slate-400">
        Loading analysis...
      </div>
    );
  }

  if (!analysis) return null;

  return (
    <div className="min-h-screen bg-midnightblue px-6 py-10">
      <div className="max-w-7xl mx-auto flex justify-between items-center mb-10">
        <div>
          <h1 className="text-3xl font-bold text-white">
            Resume Analysis
          </h1>
          <p className="text-slate-400">{analysis.resumeFileName}</p>
        </div>

        <button
          onClick={() => exportAnalysis(analysis)}
          className="px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-medium transition"
        >
          Export PDF
        </button>
      </div>

      <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="space-y-6">
          ✅ <AtsRadial score={analysis.atsScore} />

          <ResumePreview
            url={`${API_BASE}/api/resume/file/${analysis.id}`}
          />
        </div>

        <div className="lg:col-span-2 space-y-8">
          <Section title="Summary">{analysis.summary}</Section>

          <Section title="Strengths">
            <ul className="list-disc pl-5 space-y-2">
              {analysis.strengths.map((s, i) => (
                <li key={i}>{s}</li>
              ))}
            </ul>
          </Section>

          <Section title="Weaknesses">
            <ul className="list-disc pl-5 space-y-2">
              {analysis.weaknesses.map((w, i) => (
                <li key={i}>{w}</li>
              ))}
            </ul>
          </Section>

          <Section title="Improvements">
            <ul className="list-disc pl-5 space-y-2">
              {analysis.improvements.map((im, i) => (
                <li key={i}>{im}</li>
              ))}
            </ul>
          </Section>
        </div>
      </div>
    </div>
  );
}

function Section({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <div className="bg-white/5 border border-white/10 backdrop-blur rounded-2xl p-6">
      <h2 className="text-xl font-semibold text-white mb-3">
        {title}
      </h2>
      <div className="text-slate-300 leading-relaxed">
        {children}
      </div>
    </div>
  );
}

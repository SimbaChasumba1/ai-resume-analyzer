"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import axios from "axios";

import AtsRadial from "@/components/AtsRadial";

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
  const { id } = useParams<{ id: string }>();
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
        const res = await axios.get(`${API_BASE}/api/resume/analysis/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setAnalysis(res.data);
      } catch {
        router.push("/dashboard");
      } finally {
        setLoading(false);
      }
    };

    fetchAnalysis();
  }, [id, router]);

  if (loading)
    return <div className="text-center text-slate-400">Loading...</div>;

  if (!analysis) return null;

  // Compute some stats for left card
  const strengthsCount = analysis.strengths.length;
  const weaknessesCount = analysis.weaknesses.length;

  return (
    <div className="min-h-screen bg-midnightblue px-6 py-10">
      <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left Column */}
        <div className="space-y-6">
          <AtsRadial score={analysis.atsScore} />

          {/* Analysis Summary Card */}
          <div className="bg-white/5 rounded-xl p-6 flex flex-col gap-4 shadow-lg">
            <h3 className="text-lg font-semibold text-white/90">
              Resume Info
            </h3>
            <p className="text-slate-300">
              <span className="font-medium">File Name:</span> {analysis.resumeFileName}
            </p>
            <p className="text-slate-300">
              <span className="font-medium">Uploaded:</span>{" "}
              {new Date(analysis.createdAt).toLocaleString()}
            </p>

            {/* Simple Strengths vs Weaknesses bar */}
            <div className="mt-4">
              <h4 className="text-white font-medium mb-1">Analysis Breakdown</h4>
              <div className="w-full h-4 bg-white/10 rounded-full overflow-hidden flex">
                <div
                  className="bg-green-500 h-full"
                  style={{ width: `${(strengthsCount / (strengthsCount + weaknessesCount)) * 100}%` }}
                ></div>
                <div
                  className="bg-red-500 h-full"
                  style={{ width: `${(weaknessesCount / (strengthsCount + weaknessesCount)) * 100}%` }}
                ></div>
              </div>
              <div className="flex justify-between text-sm text-slate-300 mt-1">
                <span>Strengths: {strengthsCount}</span>
                <span>Weaknesses: {weaknessesCount}</span>
              </div>
            </div>
          </div>
        </div>

        {/* Right Column - Detailed Analysis */}
        <div className="lg:col-span-2 space-y-6">
          <Section title="Summary">{analysis.summary}</Section>

          <Section title="Strengths">
            <ul className="list-disc list-inside">
              {analysis.strengths.map((s, i) => (
                <li key={i}>{s}</li>
              ))}
            </ul>
          </Section>

          <Section title="Weaknesses">
            <ul className="list-disc list-inside">
              {analysis.weaknesses.map((s, i) => (
                <li key={i}>{s}</li>
              ))}
            </ul>
          </Section>

          <Section title="Improvements">
            <ul className="list-disc list-inside">
              {analysis.improvements.map((s, i) => (
                <li key={i}>{s}</li>
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
    <div className="bg-white/5 p-6 rounded-xl">
      <h2 className="text-xl font-semibold mb-3">{title}</h2>
      {children}
    </div>
  );
}


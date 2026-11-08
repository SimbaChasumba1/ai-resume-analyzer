"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import axios from "axios";

import AtsRadial from "@/components/AtsRadial";

const API_BASE =
  (process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240").replace(/\/+$/, "");

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

const POLL_INTERVAL = 2000;
const MAX_WAIT = 5 * 60 * 1000;

export default function AnalysisPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const [analysis, setAnalysis] = useState<AnalysisResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;
    let timeout: ReturnType<typeof setTimeout> | undefined;

    const startedAt = Date.now();

    const pollAnalysis = async () => {
      const token = localStorage.getItem("token");

      if (!token) {
        router.replace("/");
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

        if (cancelled) return;

        const payload = res.data?.data ?? res.data;

        const status = String(payload?.status ?? "").toLowerCase();

        const result =
          payload?.analysis ??
          payload?.result ??
          payload?.analysisResult ??
          (payload?.atsScore !== undefined ? payload : null);

        if (status === "failed") {
          setError("Resume analysis failed. Please try uploading again.");
          setLoading(false);
          return;
        }

        if (result && (status === "" || status === "completed" ||
          status === "complete" || status === "succeeded")) {
          setAnalysis({
            id: result.id,
            resumeFileName: result.resumeFileName ?? "Resume",
            summary: result.summary ?? "",
            atsScore: result.atsScore ?? 0,
            strengths: Array.isArray(result.strengths)
              ? result.strengths
              : [],
            weaknesses: Array.isArray(result.weaknesses)
              ? result.weaknesses
              : [],
            improvements: Array.isArray(result.improvements)
              ? result.improvements
              : [],
            createdAt: result.createdAt ?? new Date().toISOString(),
          });

          setLoading(false);
          return;
        }

        if (Date.now() - startedAt >= MAX_WAIT) {
          setError(
            "Your analysis is taking longer than expected. Please check your dashboard shortly."
          );
          setLoading(false);
          return;
        }

        timeout = setTimeout(pollAnalysis, POLL_INTERVAL);
      } catch (err: any) {
        if (cancelled) return;

        // A job may not be immediately available to the read endpoint.
        const statusCode = err.response?.status;

        if (
          statusCode === 404 ||
          statusCode === 202 ||
          statusCode === 204
        ) {
          if (Date.now() - startedAt < MAX_WAIT) {
            timeout = setTimeout(pollAnalysis, POLL_INTERVAL);
            return;
          }
        }

        console.error("Analysis fetch failed:", err);

        setError(
          "Unable to retrieve your analysis. Please try again shortly."
        );
        setLoading(false);
      }
    };

    pollAnalysis();

    return () => {
      cancelled = true;
      if (timeout) clearTimeout(timeout);
    };
  }, [id, router]);

  if (loading) {
    return (
      <div className="min-h-screen bg-midnightblue flex flex-col items-center justify-center text-white gap-4">
        <div className="w-10 h-10 border-4 border-indigo-400 border-t-transparent rounded-full animate-spin" />
        <h2 className="text-xl font-semibold">
          Your resume is being analyzed
        </h2>
        <p className="text-slate-400">
          This usually takes a little while. Please don't close this page.
        </p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-midnightblue flex flex-col items-center justify-center text-white gap-4 px-6">
        <h2 className="text-xl font-semibold">Analysis unavailable</h2>
        <p className="text-slate-300 text-center">{error}</p>
        <button
          onClick={() => router.push("/dashboard")}
          className="px-5 py-3 bg-indigo-600 rounded-xl hover:bg-indigo-500"
        >
          Return to Dashboard
        </button>
      </div>
    );
  }

  if (!analysis) return null;

  const strengthsCount = analysis.strengths.length;
  const weaknessesCount = analysis.weaknesses.length;
  const total = strengthsCount + weaknessesCount;

  return (
    <div className="min-h-screen bg-midnightblue px-6 py-10 text-white">
      <div className="max-w-7xl mx-auto mb-8 flex justify-between items-center">
        <h1 className="text-2xl font-bold">Resume Analysis</h1>

        <button
          onClick={() => router.push("/dashboard")}
          className="px-4 py-2 bg-white/10 rounded-lg hover:bg-white/20"
        >
          Dashboard
        </button>
      </div>

      <div className="max-w-7xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="space-y-6">
          <AtsRadial score={analysis.atsScore} />

          <div className="bg-white/5 rounded-xl p-6 flex flex-col gap-4 shadow-lg">
            <h3 className="text-lg font-semibold text-white/90">
              Resume Info
            </h3>

            <p className="text-slate-300">
              <span className="font-medium">File Name:</span>{" "}
              {analysis.resumeFileName}
            </p>

            <p className="text-slate-300">
              <span className="font-medium">Uploaded:</span>{" "}
              {new Date(analysis.createdAt).toLocaleString()}
            </p>

            <div className="mt-4">
              <h4 className="text-white font-medium mb-1">
                Analysis Breakdown
              </h4>

              <div className="w-full h-4 bg-white/10 rounded-full overflow-hidden flex">
                <div
                  className="bg-green-500 h-full"
                  style={{
                    width: total
                      ? `${(strengthsCount / total) * 100}%`
                      : "0%",
                  }}
                />

                <div
                  className="bg-red-500 h-full"
                  style={{
                    width: total
                      ? `${(weaknessesCount / total) * 100}%`
                      : "0%",
                  }}
                />
              </div>

              <div className="flex justify-between text-sm text-slate-300 mt-1">
                <span>Strengths: {strengthsCount}</span>
                <span>Weaknesses: {weaknessesCount}</span>
              </div>
            </div>
          </div>
        </div>

        <div className="lg:col-span-2 space-y-6">
          <Section title="Summary">{analysis.summary}</Section>

          <Section title="Strengths">
            <ul className="list-disc list-inside">
              {analysis.strengths.map((item, index) => (
                <li key={index}>{item}</li>
              ))}
            </ul>
          </Section>

          <Section title="Weaknesses">
            <ul className="list-disc list-inside">
              {analysis.weaknesses.map((item, index) => (
                <li key={index}>{item}</li>
              ))}
            </ul>
          </Section>

          <Section title="Improvements">
            <ul className="list-disc list-inside">
              {analysis.improvements.map((item, index) => (
                <li key={index}>{item}</li>
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
    <div className="bg-white/5 p-6 rounded-xl text-slate-200">
      <h2 className="text-xl font-semibold mb-3 text-white">{title}</h2>
      {children}
    </div>
  );
}
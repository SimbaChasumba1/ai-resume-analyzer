"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";

interface Analysis {
  resumeScore: number;
  atsScore: number;
  skills: string[];
  missingSkills: string[];
  weakPoints: string[];
  summary: string;
  recommendations: string;
}

export default function ResultsPage() {
  const { resumeId } = useParams();
  const [analysis, setAnalysis] = useState<Analysis | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchAnalysis() {
      const res = await fetch(`/api/AIAnalysis/${resumeId}`);
      const data = await res.json();
      setAnalysis(data);
      setLoading(false);
    }
    fetchAnalysis();
  }, [resumeId]);

  if (loading) return <div className="p-6">Loading analysis...</div>;
  if (!analysis) return <div className="p-6 text-red-500">No analysis found.</div>;

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <h1 className="text-3xl font-bold">Resume Analysis</h1>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Resume Score</h2>
        <p>{analysis.resumeScore}/100</p>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">ATS Compatibility</h2>
        <p>{analysis.atsScore}/100</p>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Skills Extracted</h2>
        <ul className="list-disc pl-5">
          {analysis.skills.map((skill) => (
            <li key={skill}>{skill}</li>
          ))}
        </ul>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Missing Skills</h2>
        <ul className="list-disc pl-5">
          {analysis.missingSkills.map((skill) => (
            <li key={skill}>{skill}</li>
          ))}
        </ul>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Weak Points</h2>
        <ul className="list-disc pl-5">
          {analysis.weakPoints.map((point) => (
            <li key={point}>{point}</li>
          ))}
        </ul>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Summary</h2>
        <p>{analysis.summary}</p>
      </div>

      <div className="bg-gray-50 p-4 rounded shadow">
        <h2 className="font-semibold">Recruiter Recommendations</h2>
        <p>{analysis.recommendations}</p>
      </div>

      <a
        href={`/api/AIAnalysis/${resumeId}/pdf`}
        className="inline-block mt-4 px-4 py-2 bg-green-600 text-white rounded"
      >
        Download PDF Report
      </a>
    </div>
  );
}

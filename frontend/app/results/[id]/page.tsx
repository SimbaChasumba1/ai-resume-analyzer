"use client";

import { useEffect, useState } from "react";
import axios from "axios";

export default function ResultsPage({ params }: { params: { id: string } }) {
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    const fetchAnalysis = async () => {
      try {
        const res = await axios.get(
          `http://localhost:5240/analysis/${params.id}`
        );
        setData(res.data);
      } catch (err) {
        console.error(err);
      }
    };

    fetchAnalysis();
  }, [params.id]);

  if (!data)
    return (
      <div className="min-h-screen flex items-center justify-center text-white">
        Loading analysis...
      </div>
    );

  return (
    <div className="min-h-screen py-16 px-6 flex justify-center bg-[#0D0D12] text-white">
      <div className="w-full max-w-3xl bg-[#141417] p-10 rounded-3xl border border-[#2a2a2d]">
        <h1 className="text-3xl font-bold mb-6">AI Resume Analysis</h1>

        <div className="space-y-6">
          <section>
            <h2 className="text-xl font-semibold mb-2">Summary</h2>
            <p className="text-gray-300">{data.summary}</p>
          </section>

          <section>
            <h2 className="text-xl font-semibold mb-2">Skills</h2>
            <p className="text-gray-300 whitespace-pre-line">{data.skills}</p>
          </section>

          <section>
            <h2 className="text-xl font-semibold mb-2">Weaknesses</h2>
            <p className="text-gray-300 whitespace-pre-line">
              {data.weaknesses}
            </p>
          </section>

          <section>
            <h2 className="text-xl font-semibold mb-2">Recommendations</h2>
            <p className="text-gray-300 whitespace-pre-line">
              {data.recommendations}
            </p>
          </section>
        </div>
      </div>
    </div>
  );
}

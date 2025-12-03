"use client";

export default function ResultPage({ searchParams }: any) {
  const data = searchParams.data
    ? JSON.parse(decodeURIComponent(searchParams.data))
    : null;

  return (
    <div className="pt-24 max-w-3xl mx-auto">
      <h1 className="text-3xl font-bold">Analysis Result</h1>

      {!data && (
        <p className="text-gray-500 mt-5">No analysis data found.</p>
      )}

      {data && (
        <div className="bg-white mt-6 p-6 rounded-xl shadow">
          <pre className="text-sm whitespace-pre-wrap">
            {JSON.stringify(data, null, 2)}
          </pre>
        </div>
      )}
    </div>
  );
}

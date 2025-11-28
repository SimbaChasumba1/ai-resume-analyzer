export default function AnalysisPage({ searchParams }: any) {
  const result = searchParams.result;

  return (
    <div style={{ marginTop: 150, padding: 40 }}>
      <h1>AI Resume Analysis</h1>

      <pre
        style={{
          whiteSpace: "pre-wrap",
          marginTop: 20,
          padding: 20,
          background: "#f3f3f3",
          borderRadius: 12
        }}
      >
        {result}
      </pre>
    </div>
  );
}

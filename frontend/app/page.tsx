export default function Home() {
  return (
    <div
      style={{
        marginTop: 150,
        textAlign: "center",
        padding: 40
      }}
    >
      <h1 style={{ fontSize: 38, fontWeight: 700 }}>
        AI Resume Analyzer
      </h1>

      <p style={{ fontSize: 18, marginTop: 10 }}>
        Upload your resume and get instant AI insights.
      </p>

      <a
        href="/upload"
        style={{
          marginTop: 30,
          display: "inline-block",
          padding: "14px 26px",
          background: "#2563eb",
          color: "white",
          borderRadius: 10,
          fontWeight: 600,
          textDecoration: "none"
        }}
      >
        Go to Upload Page
      </a>
    </div>
  );
}

import Link from "next/link";

export default function Home() {
  return (
    <div
      style={{
        maxWidth: "600px",
        margin: "120px auto",
        textAlign: "center",
        padding: "40px",
        borderRadius: "12px",
        backgroundColor: "white",
        boxShadow: "0 4px 20px rgba(0,0,0,0.07)",
      }}
    >
      <h1 style={{ fontSize: "32px", fontWeight: 700, marginBottom: "20px" }}>
        AI Resume Analyzer
      </h1>
      <p style={{ fontSize: "16px", color: "#555", marginBottom: "30px" }}>
        Upload your resume and get instant AI insights.
      </p>

      <Link href="/upload">
        <button
          style={{
            padding: "12px 25px",
            fontSize: "16px",
            fontWeight: 600,
            borderRadius: "8px",
            backgroundColor: "black",
            color: "white",
            cursor: "pointer",
            border: "none",
          }}
        >
          Go to Upload Page
        </button>
      </Link>
    </div>
  );
}

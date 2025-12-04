export default function ResultsPage() {
  return (
    <div className="container fade-in">
      <h1 className="gradient-text" style={{ fontSize: "2.5rem", fontWeight: 800 }}>
        Resume Analysis
      </h1>

      <div className="card" style={{ marginTop: "2rem" }}>
        <h2 style={{ fontSize: "1.6rem", marginBottom: "1rem" }}>AI Summary</h2>
        <p style={{ opacity: 0.8 }}>
          {/* Replace with AI response */}
          Your resume demonstrates strong experience in software development, with emphasis on backend architecture...
        </p>
      </div>

      <div className="card" style={{ marginTop: "2rem" }}>
        <h2 style={{ fontSize: "1.6rem", marginBottom: "1rem" }}>Skill Breakdown</h2>
        <ul style={{ opacity: 0.75, lineHeight: "1.7" }}>
          <li>✔ Node.js — Advanced</li>
          <li>✔ React — Intermediate</li>
          <li>✔ System Design — Intermediate</li>
          <li>✔ Cloud Infrastructure — Foundational</li>
        </ul>
      </div>

      <button className="btn btn-primary" style={{ marginTop: "2rem" }}>
        Download Full AI Report
      </button>
    </div>
  );
}

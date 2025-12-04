import * as React from 'react';

export default function ResumeAnalysisPage() {
  return (
    <div className="centered">
      <div className="card">
        <h2 className="title">Resume Analysis</h2>
        <p className="muted">When an uploaded resume is analyzed, results will appear here (skills, suggestions, scoring).</p>
        <div className="placeholder">No analysis yet — upload a resume to get started.</div>
      </div>
    </div>
  );
}

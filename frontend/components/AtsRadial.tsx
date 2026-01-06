"use client";

interface AtsRadialProps {
  score: number;
}

export default function AtsRadial({ score }: AtsRadialProps) {
  const radius = 60;
  const circumference = 2 * Math.PI * radius;
  const clampedScore = Math.max(0, Math.min(100, score));

  return (
    <div className="glass p-6 rounded-2xl flex flex-col items-center">
      <svg width="140" height="140" aria-label={`ATS score ${clampedScore}%`}>
        <circle
          cx="70"
          cy="70"
          r={radius}
          stroke="#1e293b"
          strokeWidth="12"
          fill="none"
        />

        <circle
          cx="70"
          cy="70"
          r={radius}
          stroke="#6366f1"
          strokeWidth="12"
          fill="none"
          strokeDasharray={circumference}
          strokeDashoffset={
            circumference - (circumference * clampedScore) / 100
          }
          strokeLinecap="round"
          transform="rotate(-90 70 70)"
        />

        <text
          x="50%"
          y="50%"
          textAnchor="middle"
          dy="8"
          fontSize="24"
          fill="white"
          fontWeight="600"
        >
          {clampedScore}%
        </text>
      </svg>

      <span className="mt-4 text-slate-300 text-sm">
        ATS Match Score
      </span>
    </div>
  );
}

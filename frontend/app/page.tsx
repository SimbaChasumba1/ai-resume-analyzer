import Link from "next/link";

export default function HomePage() {
  return (
    <div className="flex flex-col items-center justify-center text-center mt-20 px-4">
      <h1 className="text-4xl font-semibold mb-6">
        Welcome to the <span className="text-[#6A5ACD]">AI Resume Analyzer</span>
      </h1>

      <p className="text-gray-300 max-w-xl mb-10">
        Upload your resume and get instant AI-powered feedback to level up your career.
      </p>

      <Link
        href="/upload"
        className="px-6 py-3 rounded-xl bg-[#6A5ACD] hover:bg-[#7d6dff] text-white transition text-lg"
      >
        Upload Your Resume
      </Link>
    </div>
  );
}

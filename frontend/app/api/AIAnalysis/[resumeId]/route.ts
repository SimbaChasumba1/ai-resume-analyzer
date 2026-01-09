import { NextRequest, NextResponse } from "next/server";

export async function GET(
  req: NextRequest,
  { params }: { params: Promise<{ resumeId: string }> }
) {
  const { resumeId } = await params;

  const res = await fetch(
    `${process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240"}/api/resume/analysis/${resumeId}`,
    {
      headers: {
        "Content-Type": "application/json",
      },
    }
  );

  if (!res.ok) {
    return NextResponse.json(
      { error: "Failed to fetch analysis" },
      { status: res.status }
    );
  }

  const data = await res.json();
  return NextResponse.json(data);
}

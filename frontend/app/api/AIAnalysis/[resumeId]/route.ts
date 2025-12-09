import { NextResponse } from "next/server";

export async function GET(req: Request, { params }: { params: { resumeId: string } }) {
  const { resumeId } = params;
  const res = await fetch(`http://localhost:5000/api/AIAnalysis/${resumeId}`);
  const data = await res.json();
  return NextResponse.json(data);
}

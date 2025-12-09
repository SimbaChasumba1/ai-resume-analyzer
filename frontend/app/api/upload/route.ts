import { NextResponse } from "next/server";

export async function POST(req: Request) {
  const formData = await req.formData();
  const file = formData.get("file") as File;

  const backendFormData = new FormData();
  backendFormData.append("file", file);

  const res = await fetch("http://localhost:5000/api/upload", {
    method: "POST",
    body: backendFormData,
  });

  const data = await res.json();
  return NextResponse.json(data);
}

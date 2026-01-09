import { NextRequest, NextResponse } from "next/server";



type Params = {

  resumeId: string;

};



export async function GET(

  req: NextRequest,

  { params }: { params: Params }

) {

  const { resumeId } = params;



  const res = await fetch(

    `${process.env.NEXT_PUBLIC_BACKEND_URL ?? "http://localhost:5240"}/api/AIAnalysis/${resumeId}`,

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




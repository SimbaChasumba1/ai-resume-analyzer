"use client";
import { ReactNode, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

export default function AuthGuard({ children }: { children: ReactNode }) {
  const [authenticated, setAuthenticated] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) router.push("/login");
    else setAuthenticated(true);
  }, [router]);

  if (!authenticated) return <div>Redirecting...</div>;
  return <>{children}</>;
}

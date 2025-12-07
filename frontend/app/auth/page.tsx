"use client";

import AuthForm from "@/components/AuthForm";

export default function AuthPage() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50">
      <AuthForm onClose={() => {}} initialMode="login" />
    </div>
  );
}

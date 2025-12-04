import * as React from 'react';
import AuthForm from '@/components/AuthForm';

export default function AuthPage() {
  return (
    <div className="centered">
      <div className="card auth-card">
        <AuthForm />
      </div>
    </div>
  );
}


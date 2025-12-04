import * as React from 'react';
import axios from 'axios';

export default function AuthForm() {
  const [mode, setMode] = React.useState<'login'|'signup'>('login');
  const [email, setEmail] = React.useState('');
  const [password, setPassword] = React.useState('');
  const [status, setStatus] = React.useState('');

  const submit = async () => {
    if (!email || !password) return setStatus('Provide email and password');
    setStatus('Working…');
    try {
      const endpoint = mode === 'login' ? '/api/auth/login' : '/api/auth/signup';
      // Replace with your actual endpoints or frontend logic
      await axios.post(endpoint, { email, password });
      setStatus(mode === 'login' ? 'Signed in' : 'Account created');
    } catch (err) {
      console.error(err);
      setStatus('Error (backend unreachable)');
    }
  };

  return (
    <div className="auth-wrap">
      <div className="auth-toggle">
        <button className={`tab ${mode==='login'?'active':''}`} onClick={()=>setMode('login')}>Login</button>
        <button className={`tab ${mode==='signup'?'active':''}`} onClick={()=>setMode('signup')}>Sign up</button>
      </div>

      <div className="auth-form">
        <input placeholder="Email" value={email} onChange={e=>setEmail(e.target.value)} />
        <input placeholder="Password" type="password" value={password} onChange={e=>setPassword(e.target.value)} />
        <div className="actions-row">
          <button onClick={submit} className="btn primary">{mode === 'login' ? 'Sign in' : 'Create account'}</button>
        </div>
        {status && <div className="status">{status}</div>}
      </div>
    </div>
  );
}

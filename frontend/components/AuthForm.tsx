"use client";
import React, { useState } from "react";
import axios from "../utils/api";
import styled from "styled-components";

const Card = styled.div` max-width:420px;margin:20px auto;padding:18px;border-radius:10px;background:#fff;box-shadow:0 6px 20px rgba(16,24,40,0.06)`;
const Field = styled.div`margin-bottom:10px`;
const Input = styled.input`width:100%;padding:10px;border:1px solid #e6eef8;border-radius:8px`;
const Btn = styled.button`background:#2563eb;color:#fff;padding:10px 12px;border-radius:8px;border:none;cursor:pointer`;

export default function AuthForm({ mode = 'login' }: { mode?: 'login' | 'register' }) {
  const [email,setEmail] = useState('');
  const [password,setPassword] = useState('');
  const [msg,setMsg] = useState('');

  const submit = async () => {
    setMsg('Working...');
    try{
      if(mode === 'register'){
        await axios.post('/auth/register',{email,password});
        setMsg('Registered — now login');
      } else {
        const res = await axios.post('/auth/login',{email,password});
        localStorage.setItem('token', res.data.token);
        setMsg('Logged in');
        window.location.reload();
      }
    }catch(e:any){ setMsg(e?.response?.data ?? 'Error') }
  }

  return (
    <Card>
      <h3>{mode === 'login' ? 'Sign in' : 'Create account'}</h3>
      <Field><Input value={email} onChange={(e: React.ChangeEvent<HTMLInputElement>)=>setEmail(e.target.value)} placeholder="Email"/></Field>
      <Field><Input type="password" value={password} onChange={(e: React.ChangeEvent<HTMLInputElement>)=>setPassword(e.target.value)} placeholder="Password"/></Field>
      <Btn onClick={submit}>{mode === 'login' ? 'Login' : 'Register'}</Btn>
      {msg && <p style={{marginTop:10}}>{msg}</p>}
    </Card>
  );
}

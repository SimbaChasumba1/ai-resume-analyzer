import React from 'react';
import dynamic from 'next/dynamic';
const UploadForm = dynamic(() => import('../components/UploadForm'), { ssr: false });
const AuthForm = dynamic(() => import('../components/AuthForm'), { ssr: false });

export default function Home(){
  return (
    <div style={{padding:24}}>
      <h1>AI Resume Analyzer</h1>
      <div style={{display:'flex', gap:20, alignItems:'flex-start', flexWrap:'wrap'}}>
        <div style={{flex:1, minWidth:300}}>
          <AuthForm mode="login" />
        </div>
        <div style={{flex:2, minWidth:360}}>
          <UploadForm />
        </div>
      </div>
    </div>
  );
}

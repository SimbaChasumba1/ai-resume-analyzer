import type { AppProps } from 'next/app';
import React from 'react';
import Head from 'next/head';
import { createGlobalStyle } from 'styled-components';

const Global = createGlobalStyle`
  body { background: #f8fafc; font-family: Inter, ui-sans-serif, system-ui, -apple-system; margin:0 }
`;

export default function App({ Component, pageProps }: AppProps){
  return (
    <>
      <Head><title>AI Resume Analyzer</title></Head>
      <Global />
      <Component {...pageProps} />
    </>
  );
}

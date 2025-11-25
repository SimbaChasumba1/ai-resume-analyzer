import type { AppProps } from "next/app";
import Navbar from "../components/Navbar";
import "../styles/globals.css";

export default function MyApp({ Component, pageProps }: AppProps) {
  return (
    <>
      <Navbar />
      <div style={{ padding: "40px" }}>
        <Component {...pageProps} />
      </div>
    </>
  );
}

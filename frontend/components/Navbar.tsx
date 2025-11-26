"use client";
import Link from "next/link";

export default function Navbar() {
  return (
    <nav style={nav}>
      <div style={brand}>AI Resume Analyzer</div>
      <div style={right}>
        <Link href="/auth/login"><button style={btn}>Login</button></Link>
        <Link href="/auth/register"><button style={{...btn, marginLeft:8}}>Sign up</button></Link>
      </div>
    </nav>
  );
}

const nav: React.CSSProperties = {
  display: "flex",
  alignItems: "center",
  justifyContent: "space-between",
  height: "64px",
  padding: "0 20px",
  background: "#0b1220",
  color: "white",
  position: "sticky",
  top: 0,
  zIndex: 40
};

const brand: React.CSSProperties = { fontWeight: 700, fontSize: 18 };
const right: React.CSSProperties = { display: "flex", gap: 10, alignItems: "center" };
const btn: React.CSSProperties = { background: "#fff", color: "#0b1220", border: "none", padding: "8px 10px", borderRadius: 8, cursor: "pointer"};

import Link from "next/link";

export default function Navbar() {
  return (
    <nav style={styles.nav}>
      <h2 style={styles.logo}>AI Resume Analyzer</h2>

      <div>
        <Link href="/login"><button style={styles.btn}>Login</button></Link>
        <Link href="/signup"><button style={styles.btnOutline}>Sign Up</button></Link>
      </div>
    </nav>
  );
}

const styles: any = {
  nav: {
    width: "100%",
    padding: "18px 40px",
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    borderBottom: "1px solid #eee",
    position: "fixed",
    top: 0,
    background: "white",
    zIndex: 100,
  },
  logo: {
    fontSize: "20px",
    fontWeight: "700",
  },
  btn: {
    marginRight: "12px",
    padding: "8px 18px",
    background: "black",
    color: "white",
    borderRadius: "8px",
    cursor: "pointer",
    border: "none",
    fontWeight: "600",
  },
  btnOutline: {
    padding: "8px 18px",
    border: "2px solid black",
    background: "white",
    borderRadius: "8px",
    cursor: "pointer",
    fontWeight: "600",
  },
};

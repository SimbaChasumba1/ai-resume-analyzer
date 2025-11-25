import { useState } from "react";
import api from "../utils/api";
import Navbar from "../components/Navbar";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [msg, setMsg] = useState("");

  const onLogin = async () => {
    try {
      const res = await api.post("/auth/login", { email, password });
      localStorage.setItem("token", res.data.token);
      setMsg("Login successful!");
    } catch {
      setMsg("Invalid login");
    }
  };

  return (
    <>
      <Navbar />
      <div style={styles.center}>
        <div style={styles.card}>
          <h2>Login</h2>

          <input
            placeholder="Email"
            style={styles.input}
            onChange={(e) => setEmail(e.target.value)}
          />

          <input
            placeholder="Password"
            type="password"
            style={styles.input}
            onChange={(e) => setPassword(e.target.value)}
          />

          <button style={styles.button} onClick={onLogin}>
            Login
          </button>

          <p>{msg}</p>
        </div>
      </div>
    </>
  );
}

const styles: any = {
  center: {
    display: "flex",
    justifyContent: "center",
    paddingTop: "120px",
  },
  card: {
    width: "380px",
    padding: "28px",
    borderRadius: "14px",
    background: "#fff",
    boxShadow: "0 6px 18px rgba(0,0,0,0.1)",
    textAlign: "center",
  },
  input: {
    width: "100%",
    padding: "12px",
    borderRadius: "8px",
    border: "1px solid #ccc",
    marginBottom: "15px",
  },
  button: {
    width: "100%",
    padding: "12px",
    background: "black",
    color: "white",
    borderRadius: "8px",
    border: "none",
    cursor: "pointer",
    fontWeight: "600",
  },
};

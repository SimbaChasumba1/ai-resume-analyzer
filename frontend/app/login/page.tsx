export default function Login() {
  return (
    <div className="container fade-in" style={{ maxWidth: "500px", marginTop: "3rem" }}>
      <h1 className="gradient-text" style={{ fontSize: "2.3rem", fontWeight: 800 }}>
        Welcome Back
      </h1>

      <div className="card" style={{ marginTop: "2rem" }}>
        <label>Email</label>
        <input className="input" type="email" placeholder="example@mail.com" />

        <label style={{ marginTop: "1.5rem" }}>Password</label>
        <input className="input" type="password" placeholder="••••••••" />

        <button className="btn btn-primary" style={{ width: "100%", marginTop: "2rem" }}>
          Login
        </button>
      </div>
    </div>
  );
}

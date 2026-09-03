import React, { useState } from "react";
import axios from "axios";

function LoginPage({ onSwitchToSignUp }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const response = await axios.post("https://localhost:60665/api/Auth/login", {
        email: email,
        password: password,
      });
      sessionStorage.setItem("token", response.data.token);
      sessionStorage.setItem("role", response.data.role);
      localStorage.setItem("hasAccount", "true");

      window.location.reload();
    } catch (err) {
      setError("Invalid email or password.");
    }
  };

  return (
    <div>
      <h1>Login</h1>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email Address</label>
          <input
            type="email"
            placeholder="******"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
        <br />
        <div>
          <label>Password</label>
          <input
            type="password"
            placeholder="******"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        <br />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <div>
          <button type="submit">Login</button>
        </div>
      </form>
      {/* <p>
        Don't have an account?{" "}
        <button type="button" onClick={onSwitchToSignUp}>
          Sign Up
        </button>
      </p> */}
    </div>
  );
}

export default LoginPage;
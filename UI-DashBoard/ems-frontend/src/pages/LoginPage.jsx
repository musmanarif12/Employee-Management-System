import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function LoginPage({ onSwitchToSignUp, onLoginSuccess }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const res = await axios.post(`${API_BASE}/Auth/login`, { email, password });
      
      // Store token and role
      sessionStorage.setItem("token", res.data.token);
      sessionStorage.setItem("role", res.data.role);

      if (onLoginSuccess) {
        onLoginSuccess();
      } else {
        window.location.reload();
      }
    } catch (err) {
      setError("Invalid email or password.");
    }
  };

  return (
    <div>
      <h1>Login</h1>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email Address</label><br />
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <br />
        <div>
          <label>Password</label><br />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <br />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <button type="submit">Login</button>
      </form>
      <p>
        Don't have an account?{" "}
        <button type="button" onClick={onSwitchToSignUp}>
          Sign Up
        </button>
      </p>
    </div>
  );
}

export default LoginPage;
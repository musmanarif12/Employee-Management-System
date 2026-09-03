import React, { useState } from "react";
import axios from "axios";

function SignUpPage({ onSwitchToLogin }) {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSignUp = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await axios.post("https://localhost:60665/api/Auth/signup", {
        name,
        email,
        password,
      });

      localStorage.setItem("hasAccount", "true");
      onSwitchToLogin();
    } catch (err) {
      setError("Signup failed. Please try again.");
    }
  };

  return (
    <div>
      <h1>Sign Up</h1>
      <form onSubmit={handleSignUp}>
        <div>
          <label>Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <br />
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
          <button type="submit">Sign Up</button>
        </div>
      </form>
      <p>
        Already have an account?{" "}
        <button type="button" onClick={onSwitchToLogin}>
          Login
        </button>
      </p>
    </div>
  );
}

export default SignUpPage;
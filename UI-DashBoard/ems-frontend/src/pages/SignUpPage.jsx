import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function SignUpPage({ onSwitchToLogin }) {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [roleId, setRoleId] = useState(5); // Default 5 (Employee) select rahega dropdown mein
  const [reportToId, setReportToId] = useState("");
  const [error, setError] = useState("");

  const handleSignUp = async (e) => {
    e.preventDefault();
    setError("");

    // Backend 'RegisterCommand' schema ke according exact payload
    const payload = {
      fullName: fullName,
      email: email,
      password: password,
      roleId: parseInt(roleId, 10),
      reportToId: reportToId ? parseInt(reportToId, 10) : null,
    };

    try {
      await axios.post(`${API_BASE}/Auth/register`, payload);

      localStorage.setItem("hasAccount", "true");
      alert("Account created successfully! Please login.");
      onSwitchToLogin();
    } catch (err) {
      console.error("SignUp Error Details:", err.response?.data);
      if (err.response?.data?.errors) {
        const validationErrors = Object.values(err.response.data.errors)
          .flat()
          .join(" ");
        setError(validationErrors);
      } else {
        setError("Signup failed. Please check input values.");
      }
    }
  };

  return (
    <div>
      <h1>Sign Up</h1>
      <form onSubmit={handleSignUp}>
        <div>
          <label>Full Name</label>
          <br />
          <input
            type="text"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            required
          />
        </div>
        <br />
        <div>
          <label>Email Address</label>
          <br />
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <br />
        <div>
          <label>Password</label>
          <br />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <br />
        <div>
          <label>Select Role</label>
          <br />
          <select
            value={roleId}
            onChange={(e) => setRoleId(e.target.value)}
            required
          >
            <option value={5}>Employee</option>
            <option value={4}>Project Manager</option>
            <option value={3}>HR</option>
            <option value={2}>COO</option>
            <option value={1}>CEO</option>
          </select>
        </div>
        <br />
        <div>
          <label>Report To Manager ID (Optional)</label>
          <br />
          <input
            type="number"
            value={reportToId}
            onChange={(e) => setReportToId(e.target.value)}
            placeholder="e.g. 1 (Leave blank if none)"
          />
        </div>
        <br />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <button type="submit">Sign Up</button>
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
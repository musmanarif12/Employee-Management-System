import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function EmployeeDashboard() {
  const [activeView, setActiveView] = useState("home");
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const authHeader = () => ({
    headers: { Authorization: `Bearer ${sessionStorage.getItem("token")}` },
  });

  const goHome = () => {
    setActiveView("home");
    setData(null);
    setError("");
  };

  const fetchProfile = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Employees/me/profile`, authHeader());
      setData(res.data);
    } catch (err) {
      setError("Profile fetch nahi ho saka.");
    } finally {
      setLoading(false);
    }
  };

  const fetchLeaves = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Leaves/my-leaves`, authHeader());
      setData(res.data);
    } catch (err) {
      setError("Leave records fetch nahi ho sake.");
    } finally {
      setLoading(false);
    }
  };

  const fetchAttendance = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Attendance/my-history`, authHeader());
      setData(res.data);
    } catch (err) {
      setError("Attendance record fetch nahi ho saka.");
    } finally {
      setLoading(false);
    }
  };

  const handleClick = (view, fetchFn) => (e) => {
    e.preventDefault();
    setActiveView(view);
    fetchFn();
  };

  return (
    <div style={{ padding: "20px" }}>
      <h1>Employee Dashboard</h1>

      {activeView === "home" && (
        <div style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
          <div>
            <a href="#" onClick={handleClick("profile", fetchProfile)}>
              Check Personal Information
            </a>
          </div>
          <div>
            <a href="#" onClick={handleClick("leaveRequest", fetchLeaves)}>
              Check Leave Request
            </a>
          </div>
          <div>
            <a href="#" onClick={handleClick("attendance", fetchAttendance)}>
              Check Attendance Record
            </a>
          </div>
        </div>
      )}

      {activeView !== "home" && (
        <div>
          <button onClick={goHome} style={{ marginBottom: "15px" }}>
            ← Back
          </button>

          {loading && <p>Loading...</p>}
          {error && <p style={{ color: "red" }}>{error}</p>}

          {!loading && !error && activeView === "profile" && data && (
            <div>
              <h2>Personal Information</h2>
              <p><b>User ID:</b> {data.id}</p>
              <p><b>Name:</b> {data.fullName}</p>
              <p><b>Email:</b> {data.email}</p>
              <p><b>Role:</b> {data.role}</p>
            </div>
          )}

          {!loading && !error && activeView === "leaveRequest" && (
            <div>
              <h2>My Leave Requests</h2>
              {!data || data.length === 0 ? (
                <p>Koi leave request nahi mili.</p>
              ) : (
                <table border="1" cellPadding="8" style={{ borderCollapse: "collapse" }}>
                  <thead>
                    <tr>
                      <th>Leave Type</th>
                      <th>From</th>
                      <th>To</th>
                      <th>Status</th>
                      <th>Reason</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.map((leave, i) => (
                      <tr key={i}>
                        <td>{leave.leaveType}</td>
                        <td>{leave.fromDate}</td>
                        <td>{leave.toDate}</td>
                        <td>{leave.status}</td>
                        <td>{leave.reason}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          )}

          {!loading && !error && activeView === "attendance" && (
            <div>
              <h2>Attendance Record</h2>
              {!data || data.length === 0 ? (
                <p>Koi attendance record nahi mila.</p>
              ) : (
                <table border="1" cellPadding="8" style={{ borderCollapse: "collapse" }}>
                  <thead>
                    <tr>
                      <th>Date</th>
                      <th>Check In</th>
                      <th>Check Out</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.map((att, i) => (
                      <tr key={i}>
                        <td>{att.date}</td>
                        <td>{att.checkInTime ? new Date(att.checkInTime).toLocaleTimeString() : "N/A"}</td>
                        <td>{att.checkOutTime ? new Date(att.checkOutTime).toLocaleTimeString() : "N/A"}</td>
                        <td>{att.status}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default EmployeeDashboard;
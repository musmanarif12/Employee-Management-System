import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function EmployeeDashboard() {
  const [view, setView] = useState("home");
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const getHeader = () => ({
    headers: { Authorization: `Bearer ${sessionStorage.getItem("token")}` },
  });

  const handleLogout = () => {
    sessionStorage.clear();
    localStorage.removeItem("hasAccount");
    window.location.reload(); // Instantly redirects back to Login Page
  };

  const fetchData = async (targetView, url) => {
    setView(targetView);
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}${url}`, getHeader());
      setData(res.data);
    } catch (err) {
      setError("Failed to load data.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: "20px" }}>
      {/* Header section with Logout button */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: "20px",
        }}
      >
        <h1 style={{ margin: 0 }}>Employee Dashboard</h1>
        <button
          onClick={handleLogout}
          style={{
            padding: "8px 16px",
            cursor: "pointer",
            backgroundColor: "#dc3545",
            color: "#fff",
            border: "none",
            borderRadius: "4px",
          }}
        >
          Logout
        </button>
      </div>

      {view === "home" ? (
        <div style={{ display: "flex", flexDirection: "column", gap: "10px" }}>
          <button onClick={() => fetchData("profile", "/Employees/me/profile")}>
            Check Personal Information
          </button>
          <button onClick={() => fetchData("leaves", "/Leaves/my-leaves")}>
            Check Leave Requests
          </button>
          <button onClick={() => fetchData("attendance", "/Attendance/my-history")}>
            Check Attendance Record
          </button>
        </div>
      ) : (
        <div>
          <button onClick={() => setView("home")}>← Back</button>

          {loading && <p>Loading...</p>}
          {error && <p style={{ color: "red" }}>{error}</p>}

          {!loading && !error && data && (
            <>
              {/* Profile View (User ID removed) */}
              {view === "profile" && (
                <div>
                  <h2>Personal Information</h2>
                  <p><b>Name:</b> {data.fullName || data.name}</p>
                  <p><b>Email:</b> {data.email}</p>
                  <p><b>Role:</b> {data.role}</p>
                </div>
              )}

              {/* Leaves View */}
              {view === "leaves" && (
                <div>
                  <h2>My Leave Requests</h2>
                  {data.length === 0 ? (
                    <p>No leave requests found.</p>
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
                            <td>{leave.leaveType || leave.type || "N/A"}</td>
                            <td>{leave.fromDate || leave.leaveDate || "N/A"}</td>
                            <td>{leave.toDate || "N/A"}</td>
                            <td>{leave.status}</td>
                            <td>{leave.reason}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                </div>
              )}

              {/* Attendance View */}
              {view === "attendance" && (
                <div>
                  <h2>Attendance Record</h2>
                  {data.length === 0 ? (
                    <p>No attendance records found.</p>
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
                            <td>{att.date ? att.date.split("T")[0] : "N/A"}</td>
                            <td>{att.checkInTime ? new Date(att.checkInTime).toLocaleTimeString() : "N/A"}</td>
                            <td>{att.checkOutTime ? new Date(att.checkOutTime).toLocaleTimeString() : "N/A"}</td>
                            <td>{att.status || "Present"}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                </div>
              )}
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default EmployeeDashboard;
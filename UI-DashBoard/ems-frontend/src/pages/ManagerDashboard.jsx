import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function ManagerDashboard() {
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
    window.location.reload();
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

  // Fix: Manager Dashboard par role hamesha "Project Manager" show karega
  const getRoleName = () => {
    return "Project Manager";
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
        <h1 style={{ margin: 0 }}>Manager Dashboard</h1>
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
          <button onClick={() => fetchData("team", "/Employees/my-team")}>
            Check Team Members
          </button>
          <button onClick={() => fetchData("leaves", "/Leaves/pending-requests")}>
            Manage Pending Leave Requests
          </button>
        </div>
      ) : (
        <div>
          <button onClick={() => setView("home")}>← Back</button>

          {loading && <p>Loading...</p>}
          {error && <p style={{ color: "red" }}>{error}</p>}

          {!loading && !error && data && (
            <>
              {/* Profile View (User ID removed & Role forced to Project Manager) */}
              {view === "profile" && (
                <div>
                  <h2>Personal Information</h2>
                  <p><b>Name:</b> {data.fullName || data.name}</p>
                  <p><b>Email:</b> {data.email}</p>
                  <p><b>Role:</b> {getRoleName()}</p>
                </div>
              )}

              {/* Team Members View */}
              {view === "team" && (
                <div>
                  <h2>Team Members</h2>
                  {data.length === 0 ? (
                    <p>No team members assigned.</p>
                  ) : (
                    <table border="1" cellPadding="8" style={{ borderCollapse: "collapse" }}>
                      <thead>
                        <tr>
                          <th>Name</th>
                          <th>Email</th>
                          <th>Role</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.map((member, i) => (
                          <tr key={i}>
                            <td>{member.fullName || member.name}</td>
                            <td>{member.email}</td>
                            <td>{member.role || "Employee"}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                </div>
              )}

              {/* Pending Leaves View */}
              {view === "leaves" && (
                <div>
                  <h2>Pending Leave Requests</h2>
                  {data.length === 0 ? (
                    <p>No pending leave requests found.</p>
                  ) : (
                    <table border="1" cellPadding="8" style={{ borderCollapse: "collapse" }}>
                      <thead>
                        <tr>
                          <th>Employee</th>
                          <th>Leave Type</th>
                          <th>From</th>
                          <th>To</th>
                          <th>Reason</th>
                          <th>Action</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.map((leave, i) => (
                          <tr key={i}>
                            <td>{leave.employeeName || leave.fullName || "N/A"}</td>
                            <td>{leave.leaveType || leave.type || "N/A"}</td>
                            <td>{leave.fromDate || "N/A"}</td>
                            <td>{leave.toDate || "N/A"}</td>
                            <td>{leave.reason}</td>
                            <td>
                              <button style={{ marginRight: "5px" }}>Approve</button>
                              <button>Reject</button>
                            </td>
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

export default ManagerDashboard;
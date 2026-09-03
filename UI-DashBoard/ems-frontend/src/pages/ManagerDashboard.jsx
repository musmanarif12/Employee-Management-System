import React, { useState } from "react";

const ManagerDashboard = () => {
  const [view, setView] = useState("home");
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const fetchData = async (viewType, endpoint) => {
    setLoading(true);
    setError("");
    setData(null);

    const token = sessionStorage.getItem("token") || localStorage.getItem("token");

    try {
      const response = await fetch(`/api${endpoint}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to load data (Status: ${response.status})`);
      }

      const result = await response.json();
      setData(result);
      setView(viewType);
    } catch (err) {
      setError(err.message || "Failed to load data.");
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    sessionStorage.clear();
    localStorage.clear();
    window.location.reload();
  };

  const buttonStyle = {
    width: "100%",
    padding: "10px",
    backgroundColor: "#efefef",
    border: "1px solid #767676",
    borderRadius: "3px",
    fontSize: "14px",
    cursor: "pointer",
    textAlign: "center",
    boxSizing: "border-box"
  };

  return (
    <div style={{ padding: "20px 40px", fontFamily: "Times New Roman, serif" }}>
      {/* Header Section */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" }}>
        <h1 style={{ margin: 0, fontSize: "36px", fontWeight: "bold" }}>Manager Dashboard</h1>
        <button
          onClick={handleLogout}
          style={{
            backgroundColor: "#dc3545",
            color: "white",
            border: "none",
            padding: "8px 20px",
            fontSize: "16px",
            borderRadius: "4px",
            cursor: "pointer",
            fontFamily: "sans-serif"
          }}
        >
          Logout
        </button>
      </div>

      {/* Back Button */}
      {view !== "home" && (
        <button
          onClick={() => {
            setView("home");
            setError("");
            setData(null);
          }}
          style={{
            marginBottom: "15px",
            padding: "6px 16px",
            cursor: "pointer",
            fontFamily: "sans-serif"
          }}
        >
          &larr; Back
        </button>
      )}

      {error && <p style={{ color: "red", fontWeight: "bold", fontFamily: "sans-serif" }}>{error}</p>}
      {loading && <p style={{ fontFamily: "sans-serif" }}>Loading...</p>}

      {/* Home Buttons List */}
      {view === "home" && !loading && (
        <div style={{ display: "flex", flexDirection: "column", gap: "10px", width: "100%" }}>
          <button
            onClick={() => fetchData("profile", "/Employees/me/profile")}
            style={buttonStyle}
          >
            Check Personal Information
          </button>
          
          <button
            onClick={() => fetchData("leaves", "/Leaves/manager-pending-leaves")}
            style={buttonStyle}
          >
            Manage Pending Leave Requests
          </button>
        </div>
      )}

      {/* Profile View */}
      {view === "profile" && data && (
        <div style={{ marginTop: "15px", fontFamily: "sans-serif" }}>
          <h3>Personal Profile</h3>
          <p><strong>Name:</strong> {data.fullName || data.name || "N/A"}</p>
          <p><strong>Email:</strong> {data.email || "N/A"}</p>
          <p><strong>Role:</strong> Manager</p>
        </div>
      )}

      {/* Leave Requests View */}
      {view === "leaves" && (
        <div style={{ marginTop: "15px", fontFamily: "sans-serif" }}>
          <h3>Pending Leave Requests</h3>
          {Array.isArray(data) && data.length > 0 ? (
            <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
              <thead>
                <tr style={{ backgroundColor: "#f2f2f2" }}>
                  <th>Leave Date</th>
                  <th>Reason</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {data.map((leave, index) => (
                  <tr key={leave.id || leave.Id || index}>
                    <td>
                      {leave.leaveDate || leave.LeaveDate 
                        ? new Date(leave.leaveDate || leave.LeaveDate).toLocaleDateString() 
                        : "N/A"}
                    </td>
                    <td>{leave.reason || leave.Reason || "N/A"}</td>
                    <td>
                      {leave.status === 1 || leave.status === "Pending" || leave.Status === 1 || leave.Status === "Pending" 
                        ? "Pending" 
                        : leave.status || leave.Status}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p>No pending leave requests found.</p>
          )}
        </div>
      )}
    </div>
  );
};

export default ManagerDashboard;
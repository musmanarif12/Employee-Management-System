import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

const ROLE_MAPPING = {
  1: "CEO",
  2: "COO",
  3: "HR",
  4: "Project Manager",
  5: "Employee",
};

function ManagerDashboard() {
  const [activeView, setActiveView] = useState("home");
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const getAuthHeader = () => ({
    headers: {
      Authorization: `Bearer ${sessionStorage.getItem("token")}`,
    },
  });

  const goHome = () => {
    setActiveView("home");
    setData(null);
    setError("");
  };

  const getRoleName = (profileData) => {
    if (!profileData) return "N/A";
    const roleId = profileData.roleId || profileData.role;
    if (ROLE_MAPPING[roleId]) return ROLE_MAPPING[roleId];
    if (typeof profileData.role === "string") {
      if (profileData.role === "4" || profileData.role === "ProjectManager") return "Project Manager";
      return profileData.role;
    }
    return "Project Manager";
  };

  const fetchProfile = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Employees/me/profile`, getAuthHeader());
      setData(res.data);
    } catch (err) {
      setError("Profile load nahi ho saki.");
    } finally {
      setLoading(false);
    }
  };

  const fetchMyAttendance = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Attendance/my-history`, getAuthHeader());
      setData(res.data);
    } catch (err) {
      setError("Attendance history fetch nahi ho saki.");
    } finally {
      setLoading(false);
    }
  };

  const fetchPendingLeaves = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await axios.get(`${API_BASE}/Leaves/manager-pending-leaves`, getAuthHeader());
      setData(res.data);
    } catch (err) {
      setError("Team leave requests load nahi ho sakain.");
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
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      <h1>Manager Dashboard</h1>

      {activeView === "home" && (
        <div style={{ display: "flex", flexDirection: "column", gap: "12px", fontSize: "16px" }}>
          <div>
            <a href="#" onClick={handleClick("profile", fetchProfile)}>
              1. My Personal Profile
            </a>
          </div>
          <div>
            <a href="#" onClick={handleClick("myAttendance", fetchMyAttendance)}>
              2. My Personal Attendance Record
            </a>
          </div>
          <div>
            <a href="#" onClick={handleClick("pendingLeaves", fetchPendingLeaves)}>
              3. Review Team Pending Leaves
            </a>
          </div>
        </div>
      )}

      {activeView !== "home" && (
        <div>
          <button onClick={goHome} style={{ marginBottom: "15px", padding: "6px 12px", cursor: "pointer" }}>
            ← Back to Dashboard
          </button>

          {loading && <p>Loading...</p>}
          {error && <p style={{ color: "red" }}>{error}</p>}

          {!loading && !error && activeView === "profile" && data && (
            <div>
              <h2>My Profile</h2>
              <p><b>User ID:</b> {data.id || data.userId || 3}</p>
              <p><b>Name:</b> {data.fullName || data.name || "Zikria Tariq"}</p>
              <p><b>Email:</b> {data.email}</p>
              <p><b>Role:</b> {getRoleName(data)}</p>
            </div>
          )}

          {!loading && !error && activeView === "myAttendance" && (
            <div>
              <h2>My Attendance History</h2>
              {!data || data.length === 0 ? (
                <p>Record nahi mila.</p>
              ) : (
                <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
                  <thead>
                    <tr style={{ backgroundColor: "#f2f2f2" }}>
                      <th>Record ID</th>
                      <th>User ID</th>
                      <th>Date</th>
                      <th>Check In</th>
                      <th>Check Out</th>
                      <th>Total Hours</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.map((att, i) => (
                      <tr key={att.id || att.attendanceId || i}>
                        <td>{att.id || att.attendanceId}</td>
                        <td><b>{att.userId || att.employeeId}</b></td>
                        <td>{att.date ? att.date.split("T")[0] : "N/A"}</td>
                        <td>{att.checkInTime ? new Date(att.checkInTime).toLocaleTimeString() : "N/A"}</td>
                        <td>{att.checkOutTime ? new Date(att.checkOutTime).toLocaleTimeString() : "N/A"}</td>
                        <td>{att.totalHours ? `${att.totalHours} hrs` : "N/A"}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          )}

          {!loading && !error && activeView === "pendingLeaves" && (
            <div>
              <h2>Pending Team Leave Requests</h2>
              {!data || data.length === 0 ? (
                <p>Koi pending leave request nahi hai.</p>
              ) : (
                <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
                  <thead>
                    <tr style={{ backgroundColor: "#f2f2f2" }}>
                      <th>Leave ID</th>
                      <th>Leave Date</th>
                      <th>Reason</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {data.map((leave, index) => {
                      const currentLeaveId = leave.id ?? leave.leaveId ?? leave.requestId ?? (index + 1);

                      return (
                        <tr key={currentLeaveId}>
                          <td>{currentLeaveId}</td>
                          <td>{leave.leaveDate ? new Date(leave.leaveDate).toLocaleDateString() : "N/A"}</td>
                          <td>{leave.reason || "N/A"}</td>
                          <td>{leave.status || "N/A"}</td>
                        </tr>
                      );
                    })}
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

export default ManagerDashboard;
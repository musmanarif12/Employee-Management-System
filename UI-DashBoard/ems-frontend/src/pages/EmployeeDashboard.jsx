import React, { useState } from "react";
import axios from "axios";

const API_BASE = "https://localhost:60665/api";

function EmployeeDashboard() {
  const [view, setView] = useState("home");
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // Attendance Correction States
  const [selectedAttendanceId, setSelectedAttendanceId] = useState(null);
  const [reqCheckIn, setReqCheckIn] = useState("");
  const [reqCheckOut, setReqCheckOut] = useState("");
  const [reason, setReason] = useState("");
  const [submitMsg, setSubmitMsg] = useState("");

  // Leave Form States
  const [showLeaveForm, setShowLeaveForm] = useState(false);
  const [leaveType, setLeaveType] = useState("Casual");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [leaveReason, setLeaveReason] = useState("");

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
    setSelectedAttendanceId(null);
    setShowLeaveForm(false);
    setSubmitMsg("");
    try {
      const res = await axios.get(`${API_BASE}${url}`, getHeader());
      setData(res.data);
    } catch (err) {
      setError("Failed to load data.");
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCorrectionForm = (att) => {
    setSelectedAttendanceId(att.id);
    setReqCheckIn(att.checkInTime ? att.checkInTime.slice(0, 16) : "");
    setReqCheckOut(att.checkOutTime ? att.checkOutTime.slice(0, 16) : "");
    setReason("");
    setSubmitMsg("");
  };

  const handleCorrectionSubmit = async (e, attendanceId) => {
    e.preventDefault();
    setSubmitMsg("Submitting correction...");
    try {
      const payload = {
        attendanceId: attendanceId,
        requestedCheckIn: reqCheckIn,
        requestedCheckOut: reqCheckOut,
        reason: reason,
      };

      await axios.post(`${API_BASE}/Attendance/request-correction`, payload, getHeader());
      setSubmitMsg("Correction request submitted successfully!");
      setSelectedAttendanceId(null);
      fetchData("attendance", "/Attendance/my-history");
    } catch (err) {
      setSubmitMsg("Failed to submit correction request.");
    }
  };

  const handleLeaveSubmit = async (e) => {
    e.preventDefault();
    setSubmitMsg("Submitting leave request...");
    try {
      const payload = {
        leaveType: leaveType,
        fromDate: fromDate,
        toDate: toDate,
        reason: leaveReason,
      };

      await axios.post(`${API_BASE}/Leaves/apply`, payload, getHeader());
      setSubmitMsg("Leave applied successfully!");
      setShowLeaveForm(false);
      fetchData("leaves", "/Leaves/my-leaves");
    } catch (err) {
      setSubmitMsg("Failed to apply for leave.");
    }
  };

  const checkIsPending = (att) => {
    if (!att) return false;
    const rawStatus = String(att.status || att.correctionStatus || "").toLowerCase();
    
    if (rawStatus.includes("approved") || rawStatus.includes("updated") || rawStatus.includes("rejected")) {
      return false;
    }

    return (
      rawStatus.includes("correction requested") ||
      rawStatus.includes("pending") ||
      rawStatus.includes("requested") ||
      att.isCorrectionRequested === true
    );
  };

  const renderStatusBadge = (att) => {
    const rawStatus = String(att.status || att.correctionStatus || "").toLowerCase();

    if (rawStatus.includes("approved") || rawStatus.includes("updated")) {
      return (
        <span style={{ backgroundColor: "#28a745", color: "#fff", padding: "4px 8px", borderRadius: "4px", fontWeight: "bold" }}>
          Approved
        </span>
      );
    }

    if (rawStatus.includes("rejected")) {
      return (
        <span style={{ backgroundColor: "#dc3545", color: "#fff", padding: "4px 8px", borderRadius: "4px", fontWeight: "bold" }}>
          Rejected
        </span>
      );
    }

    if (checkIsPending(att)) {
      return (
        <span style={{ backgroundColor: "#ffc107", color: "#000", padding: "4px 8px", borderRadius: "4px", fontWeight: "bold" }}>
          Pending
        </span>
      );
    }

    return <span style={{ fontWeight: "bold" }}>{att.status || "Present"}</span>;
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      {/* Header */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" }}>
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
            fontWeight: "bold"
          }}
        >
          Logout
        </button>
      </div>

      {view === "home" ? (
        <div style={{ display: "flex", flexDirection: "column", gap: "10px", maxWidth: "300px" }}>
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
          <button onClick={() => setView("home")} style={{ marginBottom: "15px", cursor: "pointer" }}>
            &larr; Back
          </button>

          {loading && <p>Loading...</p>}
          {error && <p style={{ color: "red" }}>{error}</p>}

          {!loading && !error && data && (
            <>
              {/* Profile View */}
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
                  {submitMsg && <p style={{ color: "green", fontWeight: "bold" }}>{submitMsg}</p>}

                  {data.length === 0 ? (
                    <p>No leave requests found.</p>
                  ) : (
                    <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%", marginBottom: "20px" }}>
                      <thead>
                        <tr style={{ backgroundColor: "#f2f2f2" }}>
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

                  {/* Apply Leave Button & Form Under Table */}
                  <div style={{ marginTop: "15px" }}>
                    <button 
                      onClick={() => setShowLeaveForm(!showLeaveForm)}
                      style={{ padding: "8px 16px", cursor: "pointer", backgroundColor: "#007bff", color: "#fff", border: "none", borderRadius: "4px" }}
                    >
                      {showLeaveForm ? "Cancel Leave Form" : "Apply for Leave"}
                    </button>

                    {showLeaveForm && (
                      <div style={{ backgroundColor: "#f8f9fa", padding: "15px", border: "1px solid #ccc", borderRadius: "5px", marginTop: "15px", maxWidth: "400px" }}>
                        <h3>Apply New Leave</h3>
                        <form onSubmit={handleLeaveSubmit}>
                          <div>
                            <label>Leave Type: </label>
                            <select value={leaveType} onChange={(e) => setLeaveType(e.target.value)} style={{ width: "100%", padding: "5px" }}>
                              <option value="Casual">Casual</option>
                              <option value="Sick">Sick</option>
                              <option value="Annual">Annual</option>
                            </select>
                          </div>
                          <br />
                          <div>
                            <label>From Date: </label>
                            <input type="date" value={fromDate} onChange={(e) => setFromDate(e.target.value)} required style={{ width: "100%", padding: "5px" }} />
                          </div>
                          <br />
                          <div>
                            <label>To Date: </label>
                            <input type="date" value={toDate} onChange={(e) => setToDate(e.target.value)} required style={{ width: "100%", padding: "5px" }} />
                          </div>
                          <br />
                          <div>
                            <label>Reason: </label>
                            <input type="text" value={leaveReason} onChange={(e) => setLeaveReason(e.target.value)} required placeholder="Reason for leave" style={{ width: "100%", padding: "5px" }} />
                          </div>
                          <br />
                          <button type="submit" style={{ padding: "8px 16px", backgroundColor: "#28a745", color: "#fff", border: "none", borderRadius: "4px", cursor: "pointer" }}>
                            Submit Leave Request
                          </button>
                        </form>
                      </div>
                    )}
                  </div>
                </div>
              )}

              {/* Attendance View */}
              {view === "attendance" && (
                <div>
                  <h2>Attendance Record</h2>
                  {submitMsg && <p style={{ color: "green", fontWeight: "bold" }}>{submitMsg}</p>}
                  {data.length === 0 ? (
                    <p>No attendance records found.</p>
                  ) : (
                    <table border="1" cellPadding="8" style={{ borderCollapse: "collapse", width: "100%" }}>
                      <thead>
                        <tr style={{ backgroundColor: "#f2f2f2" }}>
                          <th>Date</th>
                          <th>Check In</th>
                          <th>Check Out</th>
                          <th>Status</th>
                          <th>Action</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.map((att, i) => {
                          const isPending = checkIsPending(att);

                          return (
                            <React.Fragment key={att.id || i}>
                              <tr>
                                <td>{att.date ? att.date.split("T")[0] : "N/A"}</td>
                                <td>{att.checkInTime ? new Date(att.checkInTime).toLocaleTimeString() : "N/A"}</td>
                                <td>{att.checkOutTime ? new Date(att.checkOutTime).toLocaleTimeString() : "N/A"}</td>
                                
                                <td>{renderStatusBadge(att)}</td>

                                <td>
                                  {isPending ? (
                                    <span style={{ color: "#856404", fontSize: "13px", fontWeight: "bold" }}>
                                      Request Pending
                                    </span>
                                  ) : (
                                    <button 
                                      onClick={() => handleOpenCorrectionForm(att)}
                                      style={{ padding: "4px 8px", cursor: "pointer" }}
                                    >
                                      Request Correction
                                    </button>
                                  )}
                                </td>
                              </tr>

                              {/* Form row inside Table */}
                              {selectedAttendanceId === att.id && (
                                <tr>
                                  <td colSpan="5" style={{ backgroundColor: "#f8f9fa" }}>
                                    <form onSubmit={(e) => handleCorrectionSubmit(e, att.id)}>
                                      <h4>Request Correction</h4>
                                      <div>
                                        <label>Requested Check In: </label>
                                        <input
                                          type="datetime-local"
                                          value={reqCheckIn}
                                          onChange={(e) => setReqCheckIn(e.target.value)}
                                          required
                                        />
                                      </div>
                                      <br />
                                      <div>
                                        <label>Requested Check Out: </label>
                                        <input
                                          type="datetime-local"
                                          value={reqCheckOut}
                                          onChange={(e) => setReqCheckOut(e.target.value)}
                                          required
                                        />
                                      </div>
                                      <br />
                                      <div>
                                        <label>Reason: </label>
                                        <input
                                          type="text"
                                          value={reason}
                                          onChange={(e) => setReason(e.target.value)}
                                          placeholder="Reason for correction"
                                          required
                                        />
                                      </div>
                                      <br />
                                      <button type="submit">Submit Request</button>
                                      <button type="button" onClick={() => setSelectedAttendanceId(null)} style={{ marginLeft: "5px" }}>
                                        Cancel
                                      </button>
                                    </form>
                                  </td>
                                </tr>
                              )}
                            </React.Fragment>
                          );
                        })}
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
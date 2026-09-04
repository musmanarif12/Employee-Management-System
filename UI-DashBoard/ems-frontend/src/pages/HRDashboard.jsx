import axios from "axios";
import React, { useState } from "react";

function HRDashboard() {
  const [records, setRecords] = useState([]);
  const [showRecords, setShowRecords] = useState(false);

  // States for HR Update Form
  const [editingRecordId, setEditingRecordId] = useState(null);
  const [checkInTime, setCheckInTime] = useState("");
  const [checkOutTime, setCheckOutTime] = useState("");
  const [msg, setMsg] = useState("");

  const API_BASE = "https://localhost:60665/api";

  const getHeader = () => {
    const token = sessionStorage.getItem("token");
    return {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    };
  };

  const fetchAllAttendance = async (e) => {
    if (e) e.preventDefault();
    setMsg("");
    try {
      const res = await axios.get(
        `${API_BASE}/Attendance/hr/all-records`,
        getHeader()
      );
      console.log("Fetched Records:", res.data);
      setRecords(res.data);
      setShowRecords(true);
    } catch (err) {
      console.log("Failed to fetch Attendance Record Data.");
    }
  };

  const handleOpenEditForm = (item) => {
    setEditingRecordId(item.id);
    // Requested time if available, otherwise current check-in/out time
    const initialCheckIn = item.requestedCheckIn || item.checkInTime;
    const initialCheckOut = item.requestedCheckOut || item.checkOutTime;

    setCheckInTime(initialCheckIn ? initialCheckIn.slice(0, 16) : "");
    setCheckOutTime(initialCheckOut ? initialCheckOut.slice(0, 16) : "");
    setMsg("");
  };

  const handleUpdateSubmit = async (e, attendanceId) => {
    e.preventDefault();
    setMsg("Updating...");
    try {
      const payload = {
        attendanceId: attendanceId,
        checkInTime: checkInTime,
        checkOutTime: checkOutTime,
      };

      // Updated to axios.put and endpoint path /Attendance/hr/update
      await axios.put(
        `${API_BASE}/Attendance/hr/update`,
        payload,
        getHeader()
      );

      setMsg("Attendance updated successfully!");
      setEditingRecordId(null);
      fetchAllAttendance(); // Refresh list to get updated status
    } catch (err) {
      setMsg("Failed to update attendance record.");
    }
  };

  const handleBack = () => {
    setShowRecords(false);
    setRecords([]);
    setEditingRecordId(null);
    setMsg("");
  };

  const handleLogout = () => {
    sessionStorage.clear();
    localStorage.clear();
    window.location.reload();
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial, sans-serif" }}>
      {/* Top Header */}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <h1>HR DashBoard</h1>
        <button
          onClick={handleLogout}
          style={{
            backgroundColor: "#dc3545",
            color: "white",
            border: "none",
            padding: "8px 16px",
            borderRadius: "4px",
            cursor: "pointer",
            fontWeight: "bold"
          }}
        >
          Logout
        </button>
      </div>

      {!showRecords && (
        <div style={{ marginTop: "20px" }}>
          <a href="#" onClick={fetchAllAttendance} style={{ fontSize: "18px", color: "#0056b3" }}>
            Update Attendance Record
          </a>
        </div>
      )}

      {showRecords && (
        <div style={{ marginTop: "20px" }}>
          <button 
            onClick={handleBack} 
            style={{ 
              marginBottom: "15px", 
              padding: "6px 12px", 
              cursor: "pointer", 
              backgroundColor: "#6c757d", 
              color: "#fff", 
              border: "none", 
              borderRadius: "4px" 
            }}
          >
            &larr; Back
          </button>

          <h3>Attendance Records Table</h3>
          {msg && <p><b>{msg}</b></p>}

          <table 
            border="1" 
            cellPadding="10" 
            cellSpacing="0" 
            style={{ 
              width: "100%", 
              borderCollapse: "collapse", 
              textAlign: "left",
              marginTop: "10px" 
            }}
          >
            <thead>
              <tr style={{ backgroundColor: "#f2f2f2" }}>
                <th>ID</th>
                <th>Employee Name</th>
                <th>Date</th>
                <th>Check In</th>
                <th>Check Out</th>
                <th>Status</th>
                <th>Correction Details / Reason</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {records.map((item) => {
                const isCorrectionReq = item.status?.includes("Correction") || item.requestedCheckIn || item.correctionReason;

                return (
                  <React.Fragment key={item.id}>
                    <tr 
                      style={{ 
                        backgroundColor: isCorrectionReq ? "#fff3cd" : "transparent" 
                      }}
                    >
                      <td>{item.id}</td>
                      <td><b>{item.employeeName}</b></td>
                      <td>{item.date}</td>
                      <td>{item.checkInTime}</td>
                      <td>{item.checkOutTime}</td>
                      <td>
                        <span 
                          style={{ 
                            fontWeight: "bold", 
                            padding: "4px 8px", 
                            borderRadius: "4px",
                            backgroundColor: isCorrectionReq ? "#ffc107" : "#28a745",
                            color: isCorrectionReq ? "#000" : "#fff"
                          }}
                        >
                          {item.status || (isCorrectionReq ? "Correction Requested" : "Present")}
                        </span>
                      </td>
                      <td>
                        {isCorrectionReq ? (
                          <div style={{ fontSize: "13px" }}>
                            <p style={{ margin: "2px 0" }}><b>Req Check-In:</b> {item.requestedCheckIn || "N/A"}</p>
                            <p style={{ margin: "2px 0" }}><b>Req Check-Out:</b> {item.requestedCheckOut || "N/A"}</p>
                            <p style={{ margin: "2px 0", color: "#856404" }}><b>Reason:</b> {item.correctionReason || item.reason || "N/A"}</p>
                          </div>
                        ) : (
                          <span style={{ color: "#888" }}>No Request</span>
                        )}
                      </td>
                      <td>
                        <button onClick={() => handleOpenEditForm(item)}>
                          {isCorrectionReq ? "Approve / Update" : "Edit"}
                        </button>
                      </td>
                    </tr>

                    {/* Simple HR Edit Form */}
                    {editingRecordId === item.id && (
                      <tr>
                        <td colSpan="8" style={{ backgroundColor: "#f8f9fa" }}>
                          <form onSubmit={(e) => handleUpdateSubmit(e, item.id)}>
                            <h4>Update Attendance for {item.employeeName} (ID: {item.id})</h4>
                            <div>
                              <label>New Check In: </label>
                              <input
                                type="datetime-local"
                                value={checkInTime}
                                onChange={(e) => setCheckInTime(e.target.value)}
                                required
                              />
                            </div>
                            <br />
                            <div>
                              <label>New Check Out: </label>
                              <input
                                type="datetime-local"
                                value={checkOutTime}
                                onChange={(e) => setCheckOutTime(e.target.value)}
                                required
                              />
                            </div>
                            <br />
                            <button type="submit">Confirm & Update</button>
                            <button type="button" onClick={() => setEditingRecordId(null)} style={{ marginLeft: "5px" }}>
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
        </div>
      )}
    </div>
  );
}

export default HRDashboard;
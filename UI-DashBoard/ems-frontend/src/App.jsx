import { useState } from "react";
import LoginPage from "./pages/LoginPage";
import SignUpPage from "./pages/SignUpPage";
import EmployeeDashboard from "./pages/EmployeeDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";

function App() {
  const token = sessionStorage.getItem("token");
  const role = sessionStorage.getItem("role");
  const hasAccount = localStorage.getItem("hasAccount");

  const [showSignUp, setShowSignUp] = useState(!hasAccount);

  // 1. Agar User Login nahi hai
  if (!token) {
    if (showSignUp) {
      return <SignUpPage onSwitchToLogin={() => setShowSignUp(false)} />;
    }
    return <LoginPage onSwitchToSignUp={() => setShowSignUp(true)} />;
  }

  // 2. Roles based rendering (DB IDs: 4 = ProjectManager, 5 = Employee)
  if (role === "ProjectManager" || role === "4") {
    return <ManagerDashboard />;
  }

  if (role === "Employee" || role === "5") {
    return <EmployeeDashboard />;
  }

  // Fallback agar role match na ho
  return <LoginPage onSwitchToSignUp={() => setShowSignUp(true)} />;
}

export default App;
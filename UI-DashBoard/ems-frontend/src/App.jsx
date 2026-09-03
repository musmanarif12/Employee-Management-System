import { useState } from "react";
import LoginPage from "./pages/loginpage";
import SignUpPage from "./pages/SignUpPage";
import EmployeeDashboard from "./pages/EmployeeDashboard";
import ManagerDashboard from "./pages/ManagerDashboard";

function App() {
  const token = sessionStorage.getItem("token");
  const role = sessionStorage.getItem("role");
  const hasAccount = localStorage.getItem("hasAccount");

  const [showSignUp, setShowSignUp] = useState(!hasAccount);

  if (!token) {
    // if (showSignUp) {
    //   return <SignUpPage onSwitchToLogin={() => setShowSignUp(false)} />;
    // }
    return <LoginPage onSwitchToSignUp={() => setShowSignUp(true)} />;
  }

  if (role == "Employee") {
    return <EmployeeDashboard />;
  }
  if (role == "ProjectManager") {
    return <ManagerDashboard />;
  }

  return <LoginPage onSwitchToSignUp={() => setShowSignUp(true)} />;
}

export default App;
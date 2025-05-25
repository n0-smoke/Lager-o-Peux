import '../App.css';
import {
  FaWarehouse,
  FaBoxOpen,
  FaTruck,
  FaBoxes,
  FaPowerOff,
  FaClipboardList,
  FaUserPlus,
  FaTools
} from 'react-icons/fa';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

function sendMessage(message: string) {
  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.postMessage(message);
  } else {
    alert("Not running inside WebView2.");
  }
}

function Dashboard() {
  const [activePanel, setActivePanel] = useState<string | null>(null);
  const navigate = useNavigate();

  const openPanelOrNavigate = (panel: string) => {
    setActivePanel(panel);
    // Navigate to full route (optional, depending on app design)
    // navigate(`/${panel}`);
  };

  return (
    <div className="app-container">
      <div className="background-triangles" />

      <div className="power-button" onClick={() => sendMessage("exit-app")}>
        <FaPowerOff />
      </div>

      <div className="top-icons">
        <FaUserPlus onClick={() => sendMessage("register-user")} />
        <FaClipboardList onClick={() => sendMessage("task-list")} />
        <FaTools onClick={() => sendMessage("settings")} />
      </div>

      <div className="content">
        <h1 className="main-title">Welcome to <br /> Lager-o-Peux</h1>
        <div className="icon-button-group">
          <button className="icon-button" onClick={() => openPanelOrNavigate("inventory")}>
            {activePanel !== "inventory" && <FaBoxOpen className="icon" />}
            <span>Inventory Overview</span>
          </button>
          <button className="icon-button" onClick={() => openPanelOrNavigate("warehouses")}>
            {activePanel !== "warehouses" && <FaWarehouse className="icon" />}
            <span>Warehouse Overview</span>
          </button>
          <button className="icon-button" onClick={() => openPanelOrNavigate("shipments")}>
            {activePanel !== "shipments" && <FaBoxes className="icon" />}
            <span>Shipments Overview</span>
          </button>
          <button className="icon-button" onClick={() => openPanelOrNavigate("fleet")}>
            {activePanel !== "fleet" && <FaTruck className="icon" />}
            <span>Fleet Management</span>
          </button>
        </div>
      </div>

      <div className="truck-animation-container right">
        <div className="truck-line"></div>
        <FaTruck className="truck-icon" />
        <FaTruck className="truck-icon" />
        <FaTruck className="truck-icon" />
      </div>

      <div className="truck-animation-container left">
        <div className="truck-line"></div>
        <FaTruck className="truck-icon" />
        <FaTruck className="truck-icon" />
        <FaTruck className="truck-icon" />
      </div>

      {activePanel && (
        <div className={`overlay-panel ${activePanel}`} onClick={() => setActivePanel(null)}>
          <h2>{activePanel.replace(/^\w/, c => c.toUpperCase())} Panel</h2>
          {/* Add actual panel content here */}
        </div>
      )}
    </div>
  );
}

export default Dashboard;

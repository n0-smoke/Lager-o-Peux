import './App.css';

function sendMessage(message: string) {
  if (window.chrome && window.chrome.webview) {
    window.chrome.webview.postMessage(message);
  } else {
    alert("Not running inside WebView2.");
  }
}

function App() {
  return (
    <div className="app-container">
      <h1>Inventory System</h1>
      <div className="button-group">
        <button onClick={() => sendMessage("open-shipments")}>Go to Shipments</button>
        <button onClick={() => sendMessage("open-inventory")}>Inventory Overview</button>
        <button onClick={() => sendMessage("open-warehouses")}>Warehouse Overview</button>
      </div>
    </div>
  );
}

export default App;

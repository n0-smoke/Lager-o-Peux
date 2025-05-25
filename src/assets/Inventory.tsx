import { useNavigate } from 'react-router-dom';
import { FaArrowLeft } from 'react-icons/fa';

function Inventory() {
  const navigate = useNavigate();

  return (
    <div className="page-container">
      <div className="page-header">
        <FaArrowLeft onClick={() => navigate("/")} className="back-button" />
        <h2>Inventory Overview</h2>
      </div>
      <div className="page-content">
        {/* Tu ide tvoj sadržaj */}
        <p>Pregled inventara će biti ovde.</p>
      </div>
    </div>
  );
}

export default Inventory;

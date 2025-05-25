import { useNavigate } from 'react-router-dom';
import { FaArrowLeft } from 'react-icons/fa';

function Shipments() {
  const navigate = useNavigate();

  return (
    <div className="page-container">
      <div className="page-header">
        <FaArrowLeft onClick={() => navigate("/")} className="back-button" />
        <h2>Shipments Overview</h2>
      </div>
      <div className="page-content">
        <p>Pregled pošiljki, rasporeda isporuka i njihovog statusa.</p>
      </div>
    </div>
  );
}

export default Shipments;

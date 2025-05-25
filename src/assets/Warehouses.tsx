import { useNavigate } from 'react-router-dom';
import { FaArrowLeft } from 'react-icons/fa';

function Warehouses() {
  const navigate = useNavigate();

  return (
    <div className="page-container">
      <div className="page-header">
        <FaArrowLeft onClick={() => navigate("/")} className="back-button" />
        <h2>Warehouse Overview</h2>
      </div>
      <div className="page-content">
        <p>Pregled svih skladišta, njihovih kapaciteta i zaliha.</p>
      </div>
    </div>
  );
}

export default Warehouses;

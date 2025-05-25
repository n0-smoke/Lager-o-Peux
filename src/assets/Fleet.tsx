import { useNavigate } from 'react-router-dom';
import { FaArrowLeft } from 'react-icons/fa';

function Fleet() {
  const navigate = useNavigate();

  return (
    <div className="page-container">
      <div className="page-header">
        <FaArrowLeft onClick={() => navigate("/")} className="back-button" />
        <h2>Fleet Management</h2>
      </div>
      <div className="page-content">
        <p>Upravljanje voznim parkom, dostupnost kamiona i održavanje.</p>
      </div>
    </div>
  );
}

export default Fleet;

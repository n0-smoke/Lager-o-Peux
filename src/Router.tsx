import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Dashboard from '../src/assets/Dashboard';
import Inventory from '../src/assets/Inventory';
import Warehouses from '../src/assets/Warehouses';
import Shipments from '../src/assets/Shipments';
import Fleet from '../src/assets/Fleet';
function Router() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/inventory" element={<Inventory />} />
        <Route path="/warehouses" element={<Warehouses />} />
        <Route path="/shipments" element={<Shipments />} />
        <Route path="/fleet" element={<Fleet />} />
      </Routes>
    </BrowserRouter>
  );
}

export default Router;

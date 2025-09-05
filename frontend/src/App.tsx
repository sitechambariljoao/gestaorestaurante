import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Login } from './pages/auth/Login';
import { Dashboard } from './pages/dashboard/Dashboard';
import { EmptyPage } from './pages/cadastros/EmptyPage';
import { MainLayout } from './components/layout/MainLayout';
import { PrivateRoute } from './components/auth/PrivateRoute';
import { useAuthStore } from './stores/auth';

function App() {
  const { isAuthenticated } = useAuthStore();

  return (
    <Router>
      <Routes>
        {/* Rota pública - Login */}
        <Route
          path="/login"
          element={
            isAuthenticated ? <Navigate to="/dashboard" replace /> : <Login />
          }
        />

        {/* Rotas protegidas */}
        <Route
          path="/"
          element={
            <PrivateRoute>
              <MainLayout />
            </PrivateRoute>
          }
        >
          {/* Dashboard */}
          <Route path="/dashboard" element={<Dashboard />} />
          
          {/* Cadastros - páginas vazias por enquanto */}
          <Route
            path="/cadastros/empresas"
            element={
              <EmptyPage
                title="Empresas"
                description="Gerencie as empresas do sistema"
              />
            }
          />
          <Route
            path="/cadastros/filiais"
            element={
              <EmptyPage
                title="Filiais"
                description="Gerencie as filiais das empresas"
              />
            }
          />
          <Route
            path="/cadastros/agrupamentos"
            element={
              <EmptyPage
                title="Agrupamentos"
                description="Gerencie os agrupamentos do sistema"
              />
            }
          />
          <Route
            path="/cadastros/subagrupamentos"
            element={
              <EmptyPage
                title="Sub Agrupamentos"
                description="Gerencie os sub agrupamentos do sistema"
              />
            }
          />
          <Route
            path="/cadastros/centros-custo"
            element={
              <EmptyPage
                title="Centros de Custo"
                description="Gerencie os centros de custo do sistema"
              />
            }
          />
        </Route>

        {/* Rota padrão - redireciona para dashboard ou login */}
        <Route
          path="/"
          element={
            <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />
          }
        />
      </Routes>
    </Router>
  );
}

export default App;

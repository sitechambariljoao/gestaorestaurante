import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../../stores/auth';

interface PrivateRouteProps {
  children: React.ReactNode;
}

export function PrivateRoute({ children }: PrivateRouteProps) {
  const { isAuthenticated, token } = useAuthStore();
  const location = useLocation();

  // Verificar se há token válido
  const hasValidToken = token && localStorage.getItem('auth_token');

  if (!isAuthenticated || !hasValidToken) {
    // Redirecionar para login mantendo a URL de destino
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
}
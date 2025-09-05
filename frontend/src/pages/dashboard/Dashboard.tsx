import React from 'react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui';
import { useAuthStore } from '../../stores/auth';

export function Dashboard() {
  const { user } = useAuthStore();

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-secondary-900">
          Bem-vindo ao Sistema ERP
        </h1>
        <p className="text-secondary-600 mt-2">
          Olá {user?.nome}, aqui está um resumo do seu sistema.
        </p>
      </div>

      <div className="grid gap-6 mb-8 md:grid-cols-2 xl:grid-cols-4">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-primary-100 text-primary-600">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-4m-5 0H9m11 0a2 2 0 01-2 2H7a2 2 0 01-2-2m2-16h10a2 2 0 012 2v4a2 2 0 01-2 2H7a2 2 0 01-2-2V7a2 2 0 012-2z" />
                </svg>
              </div>
              <div className="ml-4">
                <p className="text-2xl font-semibold text-secondary-900">
                  {user?.empresaNome}
                </p>
                <p className="text-sm text-secondary-600">
                  Empresa Ativa
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-success-100 text-success-600">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <div className="ml-4">
                <p className="text-2xl font-semibold text-secondary-900">
                  {user?.modulosLiberados.length}
                </p>
                <p className="text-sm text-secondary-600">
                  Módulos Liberados
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-warning-100 text-warning-600">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-4m-5 0H9m11 0a2 2 0 01-2 2H7a2 2 0 01-2-2m2-16h10a2 2 0 012 2v4a2 2 0 01-2 2H7a2 2 0 01-2-2V7a2 2 0 012-2z" />
                </svg>
              </div>
              <div className="ml-4">
                <p className="text-2xl font-semibold text-secondary-900">
                  {user?.filiaisAcesso.length}
                </p>
                <p className="text-sm text-secondary-600">
                  Filiais Acesso
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-secondary-100 text-secondary-600">
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
              <div className="ml-4">
                <p className="text-2xl font-semibold text-secondary-900">
                  {user?.perfil}
                </p>
                <p className="text-sm text-secondary-600">
                  Perfil de Acesso
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-6 mb-8 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Atalhos Rápidos</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <a href="/cadastros/empresas" className="flex items-center p-3 rounded-md hover:bg-secondary-50 transition-colors">
                <div className="p-2 bg-primary-100 rounded-md">
                  <svg className="w-4 h-4 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-4m-5 0H9m11 0a2 2 0 01-2 2H7a2 2 0 01-2-2m2-16h10a2 2 0 012 2v4a2 2 0 01-2 2H7a2 2 0 01-2-2V7a2 2 0 012-2z" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="font-medium text-secondary-900">Empresas</p>
                  <p className="text-sm text-secondary-600">Gerenciar empresas</p>
                </div>
              </a>
              <a href="/cadastros/filiais" className="flex items-center p-3 rounded-md hover:bg-secondary-50 transition-colors">
                <div className="p-2 bg-success-100 rounded-md">
                  <svg className="w-4 h-4 text-success-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-4m-5 0H9m11 0a2 2 0 01-2 2H7a2 2 0 01-2-2m2-16h10a2 2 0 012 2v4a2 2 0 01-2 2H7a2 2 0 01-2-2V7a2 2 0 012-2z" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="font-medium text-secondary-900">Filiais</p>
                  <p className="text-sm text-secondary-600">Gerenciar filiais</p>
                </div>
              </a>
              <a href="/cadastros/centros-custo" className="flex items-center p-3 rounded-md hover:bg-secondary-50 transition-colors">
                <div className="p-2 bg-warning-100 rounded-md">
                  <svg className="w-4 h-4 text-warning-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-4m-5 0H9m11 0a2 2 0 01-2 2H7a2 2 0 01-2-2m2-16h10a2 2 0 012 2v4a2 2 0 01-2 2H7a2 2 0 01-2-2V7a2 2 0 012-2z" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="font-medium text-secondary-900">Centros de Custo</p>
                  <p className="text-sm text-secondary-600">Gerenciar centros de custo</p>
                </div>
              </a>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Módulos Liberados</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {user?.modulosLiberados.map((modulo) => (
                <div key={modulo} className="flex items-center justify-between p-2 bg-success-50 rounded-md">
                  <span className="text-sm font-medium text-success-900">{modulo}</span>
                  <span className="text-xs bg-success-100 text-success-700 px-2 py-1 rounded">Ativo</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
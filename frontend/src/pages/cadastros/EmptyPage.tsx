import React from 'react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui';

interface EmptyPageProps {
  title: string;
  description: string;
}

export function EmptyPage({ title, description }: EmptyPageProps) {
  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-secondary-900">
          {title}
        </h1>
        <p className="text-secondary-600 mt-2">
          {description}
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Em Desenvolvimento</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-center py-12">
            <div className="w-16 h-16 bg-secondary-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg
                className="w-8 h-8 text-secondary-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"
                />
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                />
              </svg>
            </div>
            <h3 className="text-lg font-medium text-secondary-900 mb-2">
              Funcionalidade em Desenvolvimento
            </h3>
            <p className="text-secondary-600 max-w-md mx-auto">
              Esta página está sendo desenvolvida. Em breve você poderá gerenciar {title.toLowerCase()} 
              com funcionalidades completas de CRUD.
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
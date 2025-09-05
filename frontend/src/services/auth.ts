import { LoginCredentials, LoginResponse } from '../types';
import { apiClient } from './api';

export const authService = {
  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
    return response.data;
  },

  async logout(): Promise<void> {
    await apiClient.post('/auth/logout', {});
  },

  async getUsuarioLogado() {
    const response = await apiClient.get('/auth/usuario');
    return response.data;
  },

  async getModulosLiberados() {
    const response = await apiClient.get('/auth/modulos');
    return response.data;
  },
};
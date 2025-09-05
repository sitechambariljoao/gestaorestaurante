export interface User {
  id: string;
  nome: string;
  email: string;
  perfil: string;
  empresaId: string;
  empresaNome: string;
  modulosLiberados: string[];
  filiaisAcesso: string[];
}

export interface LoginCredentials {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  expiracao: string;
  usuario: User;
}

export interface AuthStore {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => void;
  setUser: (user: User) => void;
  setToken: (token: string) => void;
}
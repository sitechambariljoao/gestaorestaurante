import { Endereco } from './api';

export interface Empresa {
  id: string;
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  email: string;
  telefone?: string;
  endereco?: Endereco;
  ativa: boolean;
  dataCriacao: string;
}

export interface CreateEmpresaDto {
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  email: string;
  telefone?: string;
  endereco?: Endereco;
}

export interface Filial {
  id: string;
  razaoSocial: string;
  nomeFantasia: string;
  cnpj: string;
  email: string;
  telefone?: string;
  endereco?: Endereco;
  ativa: boolean;
  empresaId: string;
  dataCriacao: string;
}

export interface Agrupamento {
  id: string;
  codigo: string;
  nome: string;
  descricao?: string;
  ativo: boolean;
  filialId: string;
  dataCriacao: string;
}

export interface SubAgrupamento {
  id: string;
  codigo: string;
  nome: string;
  descricao?: string;
  ativo: boolean;
  agrupamentoId: string;
  dataCriacao: string;
}

export interface CentroCusto {
  id: string;
  codigo: string;
  nome: string;
  descricao?: string;
  ativo: boolean;
  subAgrupamentoId: string;
  dataCriacao: string;
}
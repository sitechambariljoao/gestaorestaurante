# 🎨 Frontend - Sistema ERP Restaurantes

Documentação técnica do frontend desenvolvido em React 18 + TypeScript com arquitetura moderna e design system próprio.

## 🏗️ **Arquitetura React + TypeScript**

### **Visão Geral da Arquitetura**
```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│  ┌─────────────────────────────────────────────────────┤
│  │                     Pages                           │
│  │  • Login/Auth Pages                                 │
│  │  • Dashboard                                        │
│  │  • CRUD Pages (Empresas, Filiais, etc.)            │
│  │  • Error Boundaries                                 │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                   Component Layer                       │
│  ┌─────────────────────────────────────────────────────┤
│  │                   Components                        │
│  │  • UI Components (Button, Input, Card)             │
│  │  • Layout Components (Header, Sidebar)             │
│  │  • Form Components (Validation, Masks)             │
│  │  • Auth Components (PrivateRoute)                  │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                   Business Layer                        │
│  ┌─────────────────────────────────────────────────────┤
│  │              Stores & State Management              │
│  │  • Zustand Stores (Auth, UI, Data)                 │
│  │  • React Query (Server State)                      │
│  │  • Local State (Component Level)                   │
│  └─────────────────────────────────────────────────────┤
├─────────────────────────────────────────────────────────┤
│                   Service Layer                         │
│  ┌─────────────────────────────────────────────────────┤
│  │                    Services                         │
│  │  • API Client (Axios + Interceptors)               │
│  │  • Auth Service (Login/Logout/Token)               │
│  │  • Entity Services (CRUD operations)               │
│  └─────────────────────────────────────────────────────┤
└─────────────────────────────────────────────────────────┘
```

## 🎯 **Stack Tecnológica Detalhada**

### **Core Framework**
- **React 18.3** - Framework principal com Concurrent Features
- **TypeScript 5.8** - Tipagem estática forte
- **Vite 7.1** - Build tool moderna e rápida
- **ES2022** - JavaScript moderno

### **Roteamento & Navegação**
- **React Router 6** - Client-side routing
- **History API** - Navegação programática
- **Route Guards** - Proteção de rotas autenticadas

### **Estado & Data Fetching**
- **Zustand 5.0** - State management leve e performático
- **React Query 5.86** - Server state, cache e sincronização
- **Persist Middleware** - Persistência de estado

### **Formulários & Validação**
- **React Hook Form 7.62** - Formulários performáticos
- **Zod 4.1** - Schema validation com TypeScript
- **Hookform Resolvers** - Integração RHF + Zod

### **UI & Styling**
- **TailwindCSS 4.1** - Utility-first CSS framework
- **Headless UI 2.2** - Componentes acessíveis sem estilo
- **Heroicons 2.2** - Ícones SVG otimizados
- **PostCSS + Autoprefixer** - CSS processing

### **HTTP & API**
- **Axios 1.11** - Cliente HTTP com interceptors
- **API Response Transformation** - Padronização responses

### **Build & Development**
- **Vite Dev Server** - HMR ultra-rápido
- **TypeScript Compiler** - Type checking
- **ESLint + TypeScript ESLint** - Code linting

## 📁 **Estrutura de Arquivos Detalhada**

```
frontend/src/
├── components/                 # Componentes reutilizáveis
│   ├── ui/                    # Design System base
│   │   ├── Button.tsx         # Componente botão com variants
│   │   ├── Input.tsx          # Input com validação visual
│   │   ├── Card.tsx           # Container padrão conteúdo
│   │   └── index.ts           # Barrel exports
│   ├── layout/                # Componentes de layout
│   │   ├── Header.tsx         # Cabeçalho com user menu
│   │   ├── Sidebar.tsx        # Menu lateral colapsável
│   │   └── MainLayout.tsx     # Layout principal wrapper
│   ├── forms/                 # Componentes de formulário
│   │   ├── EmpresaForm.tsx    # Formulário empresas
│   │   └── ValidationWrapper.tsx # Wrapper validação
│   └── auth/                  # Componentes autenticação
│       ├── PrivateRoute.tsx   # Guard para rotas protegidas
│       └── RoleGuard.tsx      # Guard por perfil/módulo
├── pages/                     # Páginas da aplicação
│   ├── auth/                  # Páginas autenticação
│   │   ├── Login.tsx          # Tela de login
│   │   └── Register.tsx       # Registro (futuro)
│   ├── dashboard/             # Dashboard principal
│   │   └── Dashboard.tsx      # Métricas e atalhos
│   └── cadastros/             # Páginas CRUD
│       ├── empresas/          # Gestão empresas
│       ├── filiais/           # Gestão filiais
│       └── EmptyPage.tsx      # Placeholder desenvolvimento
├── stores/                    # Gerenciamento de estado
│   ├── auth.ts               # Store autenticação
│   ├── ui.ts                 # Store interface (modals, toasts)
│   └── data.ts               # Cache local entidades
├── services/                  # Serviços e integrações
│   ├── api.ts                # Cliente API base (Axios)
│   ├── auth.ts               # Serviços autenticação
│   ├── empresa.ts            # CRUD empresas
│   └── index.ts              # Barrel exports
├── hooks/                     # Custom React hooks
│   ├── useAuth.ts            # Hook autenticação
│   ├── useApi.ts             # Hook API calls
│   └── useLocalStorage.ts    # Hook localStorage
├── types/                     # Definições TypeScript
│   ├── auth.ts               # Types autenticação
│   ├── entities.ts           # Types entidades negócio
│   ├── api.ts                # Types API responses
│   └── index.ts              # Barrel exports
├── utils/                     # Utilitários e helpers
│   ├── cn.ts                 # Class name utility (clsx)
│   ├── formatters.ts         # Formatação CNPJ, telefone, etc.
│   ├── validators.ts         # Validações customizadas
│   └── constants.ts          # Constantes aplicação
├── styles/                    # CSS e temas
│   └── globals.css           # Styles globais + Tailwind
└── assets/                   # Assets estáticos
    ├── icons/                # Ícones customizados
    └── images/               # Imagens aplicação
```

## 🎨 **Design System & UI Components**

### **Filosofia de Design**
- **Mobile-First**: Design responsivo começando mobile
- **Accessibility**: WCAG 2.1 AA compliance
- **Consistency**: Componentes padronizados reutilizáveis
- **Performance**: Lazy loading e otimizações

### **Color Palette**
```typescript
const colors = {
  primary: {
    50: '#eff6ff',
    500: '#3b82f6',  // Azul principal - ações primárias
    600: '#2563eb',
    900: '#1e3a8a'
  },
  secondary: {
    50: '#f9fafb', 
    500: '#6b7280',  // Cinza - ações secundárias
    900: '#111827'
  },
  success: {
    500: '#10b981',  // Verde - confirmações/sucesso
  },
  warning: {
    500: '#f59e0b',  // Amarelo - avisos
  },
  danger: {
    500: '#ef4444',  // Vermelho - ações críticas
  }
}
```

### **Typography Scale**
```typescript
const typography = {
  fontFamily: {
    sans: ['Inter', 'system-ui', 'sans-serif'],
    mono: ['JetBrains Mono', 'monospace']
  },
  fontSize: {
    xs: '0.75rem',    // 12px
    sm: '0.875rem',   // 14px  
    base: '1rem',     // 16px
    lg: '1.125rem',   // 18px
    xl: '1.25rem',    // 20px
    '2xl': '1.5rem',  // 24px
    '3xl': '1.875rem' // 30px
  },
  fontWeight: {
    normal: 400,
    medium: 500,
    semibold: 600,
    bold: 700
  }
}
```

### **Component Variants System**
```typescript
// Button Component Example
interface ButtonProps {
  variant: 'primary' | 'secondary' | 'ghost' | 'danger';
  size: 'sm' | 'md' | 'lg';
  loading?: boolean;
}

const Button = ({ variant = 'primary', size = 'md', loading = false, ...props }) => {
  const baseStyles = 'inline-flex items-center justify-center rounded-md font-medium transition-colors';
  
  const variants = {
    primary: 'bg-primary-600 text-white hover:bg-primary-700',
    secondary: 'bg-secondary-200 text-secondary-900 hover:bg-secondary-300',
    ghost: 'text-secondary-700 hover:bg-secondary-100',
    danger: 'bg-danger-600 text-white hover:bg-danger-700'
  };
  
  const sizes = {
    sm: 'h-8 px-3 text-sm',
    md: 'h-10 px-4 text-sm', 
    lg: 'h-12 px-6 text-base'
  };
  
  return (
    <button className={cn(baseStyles, variants[variant], sizes[size])}>
      {loading && <LoadingSpinner />}
      {children}
    </button>
  );
};
```

## 🔐 **Sistema de Autenticação**

### **Auth Store (Zustand)**
```typescript
interface AuthStore {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => void;
  setUser: (user: User) => void;
  setToken: (token: string) => void;
}

export const useAuthStore = create<AuthStore>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      
      login: async (credentials) => {
        const response = await authService.login(credentials);
        set({
          user: response.usuario,
          token: response.token,
          isAuthenticated: true
        });
        localStorage.setItem('auth_token', response.token);
      },
      
      logout: async () => {
        await authService.logout();
        set({ user: null, token: null, isAuthenticated: false });
        localStorage.removeItem('auth_token');
      }
    }),
    { name: 'auth-storage' }
  )
);
```

### **Protected Routes**
```typescript
function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, token } = useAuthStore();
  const location = useLocation();
  
  const hasValidToken = token && localStorage.getItem('auth_token');
  
  if (!isAuthenticated || !hasValidToken) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  
  return <>{children}</>;
}
```

### **API Client com Interceptors**
```typescript
class ApiClient {
  private client: AxiosInstance;
  
  constructor() {
    this.client = axios.create({
      baseURL: 'http://localhost:5268/api',
      headers: { 'Content-Type': 'application/json' }
    });
    
    this.setupInterceptors();
  }
  
  private setupInterceptors() {
    // Request interceptor - adicionar token
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('auth_token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
    
    // Response interceptor - tratamento erros
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token expirado - redirect login
          localStorage.removeItem('auth_token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }
}
```

## 🧭 **Sistema de Roteamento**

### **Estrutura de Rotas**
```typescript
function App() {
  const { isAuthenticated } = useAuthStore();
  
  return (
    <BrowserRouter>
      <Routes>
        {/* Rota pública */}
        <Route 
          path="/login" 
          element={isAuthenticated ? <Navigate to="/dashboard" /> : <Login />} 
        />
        
        {/* Rotas protegidas */}
        <Route path="/" element={<PrivateRoute><MainLayout /></PrivateRoute>}>
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="cadastros/empresas" element={<EmpresasPage />} />
          <Route path="cadastros/filiais" element={<FiliaisPage />} />
          {/* ... outras rotas */}
        </Route>
        
        {/* Redirect padrão */}
        <Route 
          path="/" 
          element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} />} 
        />
      </Routes>
    </BrowserRouter>
  );
}
```

### **Navegação Programática**
```typescript
function useNavigation() {
  const navigate = useNavigate();
  const location = useLocation();
  
  const goToPage = (path: string, options?: NavigateOptions) => {
    navigate(path, options);
  };
  
  const goBack = () => {
    navigate(-1);
  };
  
  const getCurrentPath = () => location.pathname;
  
  return { goToPage, goBack, getCurrentPath };
}
```

## 📦 **Gerenciamento de Estado**

### **Zustand Stores Strategy**
```typescript
// Auth Store - Global authentication state
const useAuthStore = create<AuthStore>(/* auth logic */);

// UI Store - Interface state (modals, toasts, loading)
const useUIStore = create<UIStore>((set) => ({
  isLoading: false,
  modals: {
    createEmpresa: false,
    deleteConfirm: false
  },
  toasts: [],
  setLoading: (loading: boolean) => set({ isLoading: loading }),
  openModal: (modalName: string) => set(state => ({
    modals: { ...state.modals, [modalName]: true }
  })),
  showToast: (toast: Toast) => set(state => ({
    toasts: [...state.toasts, toast]
  }))
}));

// Data Store - Local cache for entities
const useDataStore = create<DataStore>((set) => ({
  empresas: [],
  filiais: [],
  lastSync: null,
  setEmpresas: (empresas) => set({ empresas, lastSync: new Date() }),
  addEmpresa: (empresa) => set(state => ({
    empresas: [...state.empresas, empresa]
  }))
}));
```

### **React Query Integration**
```typescript
// Custom hooks for server state
export function useEmpresas() {
  return useQuery({
    queryKey: ['empresas'],
    queryFn: () => empresaService.getAll(),
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });
}

export function useCreateEmpresa() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateEmpresaDto) => empresaService.create(data),
    onSuccess: () => {
      // Invalidate and refetch empresas list
      queryClient.invalidateQueries(['empresas']);
      showSuccessToast('Empresa criada com sucesso!');
    },
    onError: (error) => {
      showErrorToast('Erro ao criar empresa');
    }
  });
}
```

## 📋 **Formulários & Validação**

### **React Hook Form + Zod Integration**
```typescript
// Zod Schema
const empresaSchema = z.object({
  razaoSocial: z.string()
    .min(1, 'Razão social é obrigatória')
    .max(255, 'Máximo 255 caracteres'),
  nomeFantasia: z.string()
    .min(1, 'Nome fantasia é obrigatório')
    .max(255, 'Máximo 255 caracteres'),
  cnpj: z.string()
    .min(14, 'CNPJ inválido')
    .refine(validateCNPJ, 'CNPJ inválido'),
  email: z.string()
    .email('Email inválido')
    .max(255, 'Máximo 255 caracteres')
});

type EmpresaFormData = z.infer<typeof empresaSchema>;

// Form Component
function EmpresaForm({ onSubmit }: EmpresaFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<EmpresaFormData>({
    resolver: zodResolver(empresaSchema)
  });
  
  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Input
        label="Razão Social"
        error={errors.razaoSocial?.message}
        {...register('razaoSocial')}
      />
      <Input
        label="CNPJ"
        error={errors.cnpj?.message}
        {...register('cnpj')}
        onInput={maskCNPJ} // Auto-formatting
      />
      <Button type="submit" loading={isSubmitting}>
        Salvar
      </Button>
    </form>
  );
}
```

### **Máscaras e Formatação**
```typescript
// CNPJ Mask
export function maskCNPJ(value: string): string {
  return value
    .replace(/\D/g, '')
    .replace(/(\d{2})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1/$2')
    .replace(/(\d{4})(\d)/, '$1-$2')
    .substring(0, 18);
}

// Telefone Mask
export function maskTelefone(value: string): string {
  return value
    .replace(/\D/g, '')
    .replace(/(\d{2})(\d)/, '($1) $2')
    .replace(/(\d{5})(\d)/, '$1-$2')
    .substring(0, 15);
}
```

## 🔧 **Utilitários & Helpers**

### **Class Name Utility**
```typescript
// cn.ts - Tailwind class merging
import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

// Usage
<Button className={cn(
  'base-button-styles',
  variant === 'primary' && 'primary-styles',
  disabled && 'disabled-styles'
)} />
```

### **API Response Types**
```typescript
export interface ApiResponse<T = any> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
```

## 🚧 **Desenvolvimento & Build**

### **Scripts Disponíveis**
```json
{
  "scripts": {
    "dev": "vite",                    // Desenvolvimento com HMR
    "build": "tsc -b && vite build",  // Build produção
    "lint": "eslint .",               // Análise código
    "preview": "vite preview"         // Preview build local
  }
}
```

### **Configuração Vite**
```typescript
// vite.config.ts
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5268',
        changeOrigin: true,
        secure: false
      }
    }
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          router: ['react-router-dom'],
          ui: ['@headlessui/react', '@heroicons/react']
        }
      }
    }
  }
});
```

## 📊 **Performance & Otimizações**

### **Code Splitting**
```typescript
// Lazy loading pages
const Dashboard = lazy(() => import('../pages/dashboard/Dashboard'));
const EmpresasPage = lazy(() => import('../pages/cadastros/EmpresasPage'));

// Suspense wrapper
<Suspense fallback={<LoadingSpinner />}>
  <Dashboard />
</Suspense>
```

### **Bundle Analysis**
```bash
# Analisar bundle size
npm run build -- --analyze

# Principais chunks esperados:
# - vendor.js (React, libs)
# - app.js (código aplicação)
# - router.js (React Router)
# - ui.js (componentes UI)
```

## 🧪 **Testing Strategy (Futuro)**

### **Ferramentas Planejadas**
- **Vitest** - Test runner rápido
- **Testing Library** - Component testing
- **MSW** - Mock Service Worker para API
- **Playwright** - E2E testing

### **Estrutura de Testes**
```typescript
// Component test example
describe('Button Component', () => {
  it('renders with correct variant classes', () => {
    render(<Button variant="primary">Test</Button>);
    expect(screen.getByRole('button')).toHaveClass('bg-primary-600');
  });
  
  it('shows loading spinner when loading prop is true', () => {
    render(<Button loading>Test</Button>);
    expect(screen.getByTestId('loading-spinner')).toBeInTheDocument();
  });
});
```

## 🚀 **Próximos Passos**

### **🚧 Em Desenvolvimento**
1. **CRUD Pages** - Implementar páginas completas empresas/filiais
2. **Form Builder** - Sistema de formulários dinâmicos
3. **Data Tables** - Tabelas com sort/filter/pagination
4. **Toast System** - Notificações e feedback

### **🔮 Roadmap**
1. **PWA** - Progressive Web App capabilities
2. **Dark Mode** - Tema escuro completo
3. **Internationalization** - Suporte múltiplos idiomas
4. **Mobile App** - React Native version

---

**Interface moderna, performática e preparada para escala enterprise.**
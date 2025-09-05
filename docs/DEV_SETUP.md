# 🚀 Setup Ambiente de Desenvolvimento

Este guia detalha como configurar o ambiente de desenvolvimento para o Sistema ERP Restaurantes.

## 📋 **Pré-requisitos**

### **Software Obrigatório**
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+** - [Download](https://nodejs.org/) (recomendado 18.x ou superior)
- **npm** - Geralmente incluído com Node.js
- **SQL Server** - LocalDB, Express ou Full (SQL Server 2019+ recomendado)
- **Git** - [Download](https://git-scm.com/)

### **IDEs Recomendadas**
- **Visual Studio 2022** (Windows) ou **Visual Studio Code**
- **Extensions VS Code recomendadas**:
  - C# Dev Kit
  - TypeScript e JavaScript Language Features
  - ES7+ React/Redux/React-Native snippets
  - Tailwind CSS IntelliSense

### **Ferramentas Opcionais**
- **SQL Server Management Studio (SSMS)** - Para gerenciar database
- **Postman** - Para testar APIs
- **Docker** - Para containerização (futuro)

## 🎯 **Backend Setup (.NET 9.0)**

### **1. Verificar Instalação .NET**
```bash
dotnet --version
# Deve retornar 9.0.x
```

### **2. Clonar e Navegar para Backend**
```bash
git clone [repository-url]
cd gestaorestaurante/backend/src
```

### **3. Restaurar Dependências**
```bash
# Restaurar todos os projetos
dotnet restore

# Verificar se build está funcionando
dotnet build
```

### **4. Configurar Database**

#### **4.1 String de Conexão**
Edite `backend/src/GestaoRestaurante.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DESKTOP-GSVD334;Initial Catalog=GestaoRestaurante;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-super-segura-aqui",
    "Issuer": "GestaoRestauranteAPI",
    "Audience": "GestaoRestauranteFrontend",
    "ExpirationInHours": 8
  }
}
```

> **Nota**: Ajuste a connection string conforme sua instalação do SQL Server

#### **4.2 Executar Migrations**
```bash
# Navegar para diretório da API
cd GestaoRestaurante.API

# Executar migrations
dotnet ef database update --project ../GestaoRestaurante.Infrastructure --startup-project .

# Verificar se database foi criado
# Database: GestaoRestaurante
```

#### **4.3 Verificar Seed Data**
O sistema possui dados iniciais (seed data) que são inseridos automaticamente:
- Usuário admin: `admin@restaurantedemo.com` / `Admin123!`
- Empresa demo com estrutura completa
- Planos de assinatura

### **5. Executar Backend**
```bash
# Estar no diretório GestaoRestaurante.API
dotnet run

# Saída esperada:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5268
# info: Microsoft.Hosting.Lifetime[0]
#       Application started. Press Ctrl+C to shutdown.
```

### **6. Verificar Backend Funcionando**
- **API Base**: http://localhost:5268
- **Swagger UI**: http://localhost:5268/swagger
- **Health Check**: http://localhost:5268/api/health

### **7. Executar Testes**
```bash
# Voltar para diretório backend/src
cd ..
dotnet test GestaoRestaurante.Tests/

# Saída esperada: 42/46 testes passando (91% success rate)
```

## 🎨 **Frontend Setup (React + TypeScript)**

### **1. Verificar Instalação Node.js**
```bash
node --version
# Deve retornar v18.x.x ou superior

npm --version
# Deve retornar 8.x.x ou superior
```

### **2. Navegar para Frontend**
```bash
# A partir da raiz do projeto
cd frontend
```

### **3. Instalar Dependências**
```bash
npm install

# Instalação pode levar alguns minutos
# Aguardar até ver "added XXX packages"
```

### **4. Verificar Dependências Instaladas**
As principais dependências que devem estar instaladas:

**Produção**:
- React 18 + TypeScript
- React Router 6
- Zustand (state management)
- React Hook Form + Zod
- Axios
- TailwindCSS + Headless UI

**Desenvolvimento**:
- Vite
- ESLint + TypeScript ESLint
- Tailwind + PostCSS

### **5. Executar Frontend**
```bash
npm run dev

# Saída esperada:
# VITE v7.1.4  ready in XXX ms
# ➜  Local:   http://localhost:5173/
# ➜  Network: use --host to expose
```

### **6. Verificar Frontend Funcionando**
- **URL**: http://localhost:5173
- **Tela Inicial**: Login page
- **Credenciais**: `admin@restaurantedemo.com` / `Admin123!`

### **7. Build de Produção (Opcional)**
```bash
# Build para produção
npm run build

# Preview do build
npm run preview
```

## 🧪 **Executar Testes**

### **Backend Tests**
```bash
cd backend/src
dotnet test GestaoRestaurante.Tests/ --verbosity normal

# Testes por categoria:
# - EmpresaControllerTests: 15 testes
# - AuthControllerTests: 21 testes  
# - HealthControllerTests: 6 testes
```

### **Frontend Tests (Futuro)**
```bash
cd frontend
npm test
# Testes ainda não implementados
```

## 🔧 **Configurações de Desenvolvimento**

### **VS Code Settings**
Crie `.vscode/settings.json` na raiz:

```json
{
  "dotnet.defaultSolution": "backend/src/GestaoRestaurante.sln",
  "typescript.preferences.includePackageJsonAutoImports": "on",
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  },
  "emmet.includeLanguages": {
    "typescript": "html",
    "typescriptreact": "html"
  }
}
```

### **Variáveis de Ambiente**
Crie `frontend/.env.local`:

```env
VITE_API_URL=http://localhost:5268/api
VITE_APP_NAME="Sistema ERP Restaurantes"
VITE_APP_VERSION="1.0.0"
```

## 🐛 **Troubleshooting Comum**

### **Erro: Database Connection**
```
A network-related or instance-specific error occurred
```
**Solução**: Verificar se SQL Server está rodando e ajustar connection string

### **Erro: Port 5268 Already in Use**
```
Failed to bind to address http://localhost:5268
```
**Solução**: 
```bash
# Matar processo na porta
netstat -ano | findstr :5268
taskkill /PID [PID_NUMBER] /F
```

### **Erro: npm install fails**
```
npm ERR! network
```
**Solução**:
```bash
# Limpar cache npm
npm cache clean --force

# Tentar novamente
npm install
```

### **Erro: Migration fails**
```
Unable to create an object of type 'ApplicationDbContext'
```
**Solução**: Verificar connection string e SQL Server rodando

## ✅ **Checklist Final**

Após seguir todos os passos, verificar:

- [ ] **Backend rodando** em http://localhost:5268
- [ ] **Swagger acessível** em http://localhost:5268/swagger
- [ ] **Frontend rodando** em http://localhost:5173
- [ ] **Login funcionando** com credenciais admin
- [ ] **Database criado** com nome `GestaoRestaurante`
- [ ] **Testes backend passando** (mínimo 40/46)

## 📞 **Suporte**

Em caso de dúvidas:

1. Consulte [Troubleshooting](./TROUBLESHOOTING.md)
2. Verifique logs no console
3. Consulte documentação específica:
   - [Backend Docs](./backend/README.md)
   - [Frontend Docs](./frontend/README.md)

---

**🎉 Setup concluído! Agora você pode começar a desenvolver!**
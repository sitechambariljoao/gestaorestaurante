# 🛠️ Troubleshooting - Sistema ERP Restaurantes

Guia de solução de problemas comuns do sistema.

## 🚨 **Problemas Críticos**

### **❌ Erro: Sistema não inicia**

#### **Backend não inicializa**
```bash
# Error: Unable to start application
# Build failed
```

**Soluções**:
1. **Verificar .NET SDK**:
   ```bash
   dotnet --version
   # Deve retornar 9.0.x
   ```

2. **Restaurar packages**:
   ```bash
   cd backend/src
   dotnet restore
   dotnet build
   ```

3. **Verificar connection string**:
   - Editar `appsettings.json`
   - Confirmar SQL Server rodando
   - Testar conexão manualmente

#### **Frontend não inicializa**
```bash
# Error: Cannot resolve dependency
# Port already in use
```

**Soluções**:
1. **Limpar node_modules**:
   ```bash
   cd frontend
   rm -rf node_modules package-lock.json
   npm install
   ```

2. **Verificar Node.js**:
   ```bash
   node --version  # >= 18.x
   npm --version   # >= 8.x
   ```

3. **Liberar porta 5173**:
   ```bash
   # Windows
   netstat -ano | findstr :5173
   taskkill /PID [PID] /F
   
   # Linux/Mac
   lsof -ti:5173 | xargs kill
   ```

---

## 🔐 **Problemas de Autenticação**

### **❌ Login não funciona**

#### **"Invalid credentials"**
**Causas Possíveis**:
- Credenciais incorretas
- Usuário inativo/bloqueado
- Database não sincronizado

**Soluções**:
1. **Verificar credenciais padrão**:
   - Email: `admin@restaurantedemo.com`
   - Senha: `Admin123!`

2. **Verificar database**:
   ```sql
   SELECT Id, Email, LockoutEnd, EmailConfirmed
   FROM AspNetUsers 
   WHERE Email = 'admin@restaurantedemo.com';
   ```

3. **Reset senha (admin)**:
   ```bash
   # Executar seed novamente
   cd backend/src/GestaoRestaurante.API
   dotnet run --seed
   ```

#### **Token expira muito rápido**
**Solução**: Verificar `appsettings.json`:
```json
{
  "JwtSettings": {
    "ExpirationInHours": 8
  }
}
```

#### **"401 Unauthorized" após login**
**Causas**:
- Token malformado
- Chave secreta incorreta
- Clock skew entre cliente/servidor

**Soluções**:
1. **Verificar JWT secret**:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "sua-chave-deve-ter-pelo-menos-256-bits"
     }
   }
   ```

2. **Limpar localStorage**:
   ```javascript
   localStorage.clear();
   sessionStorage.clear();
   ```

---

## 💾 **Problemas de Database**

### **❌ Connection String Issues**

#### **"Cannot connect to SQL Server"**
**Soluções**:
1. **Verificar SQL Server rodando**:
   ```bash
   # Windows - Services
   services.msc → SQL Server (MSSQLSERVER)
   
   # Ou via SQL Server Configuration Manager
   ```

2. **Connection string correta**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=GestaoRestaurante;Integrated Security=True;TrustServerCertificate=true"
     }
   }
   ```

3. **Verificar firewall**:
   - Porta 1433 liberada
   - SQL Server Browser ativo
   - TCP/IP habilitado

### **❌ Migration Issues**

#### **"Pending migrations"**
```bash
dotnet ef migrations list --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
```

**Soluções**:
1. **Aplicar migrations**:
   ```bash
   dotnet ef database update --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
   ```

2. **Reset completo** (CUIDADO - perde dados):
   ```bash
   dotnet ef database drop --force --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
   dotnet ef database update --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
   ```

#### **"Migration already applied"**
**Solução**:
```bash
# Verificar status
dotnet ef migrations list

# Reverter para migration anterior
dotnet ef database update PreviousMigrationName
```

---

## 🌐 **Problemas de Conectividade**

### **❌ CORS Errors**

#### **"Access to fetch blocked by CORS"**
**Solução**: Verificar `Program.cs`:
```csharp
app.UseCors(policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
```

### **❌ API não responde**

#### **Timeout ou 502 Bad Gateway**
**Soluções**:
1. **Verificar processo**:
   ```bash
   netstat -ano | findstr :5268
   ```

2. **Logs do Kestrel**:
   ```bash
   cd backend/src/GestaoRestaurante.API
   dotnet run --verbosity detailed
   ```

3. **Health check**:
   ```bash
   curl http://localhost:5268/api/health
   ```

---

## 📊 **Problemas de Performance**

### **❌ Queries lentas**

#### **Identificar queries problemáticas**
```sql
-- Top 10 queries mais lentas
SELECT TOP 10
    qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
    qs.execution_count,
    SUBSTRING(qt.text, qs.statement_start_offset/2, 
        (CASE WHEN qs.statement_end_offset = -1 
         THEN LEN(CONVERT(NVARCHAR(max), qt.text)) * 2 
         ELSE qs.statement_end_offset END - qs.statement_start_offset)/2) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_elapsed_time DESC;
```

#### **Otimizações**:
1. **Rebuild indexes**:
   ```sql
   ALTER INDEX ALL ON Produtos REBUILD;
   ALTER INDEX ALL ON Empresas REBUILD;
   ```

2. **Update statistics**:
   ```sql
   UPDATE STATISTICS Produtos;
   UPDATE STATISTICS Empresas;
   ```

### **❌ Memory Issues**

#### **High memory usage**
**Soluções**:
1. **Configurar EF tracking**:
   ```csharp
   context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
   ```

2. **Configurar connection pooling**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "...;Max Pool Size=100;Min Pool Size=10;"
     }
   }
   ```

---

## 🎨 **Problemas de Frontend**

### **❌ Componentes não renderizam**

#### **Erro de compilação TypeScript**
```bash
# Verificar tipos
npm run type-check

# Limpar cache TypeScript
rm -rf node_modules/.cache
```

#### **Erro de Tailwind**
**Verificar `tailwind.config.js`**:
```javascript
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  // ...
}
```

### **❌ Estado não persiste**

#### **Zustand não salva**
**Verificar persist middleware**:
```typescript
export const useAuthStore = create<AuthStore>()(
  persist(
    (set, get) => ({
      // store logic
    }),
    {
      name: 'auth-storage', // localStorage key
      partialize: (state) => ({
        user: state.user,
        token: state.token
      })
    }
  )
);
```

---

## 🔧 **Problemas de Configuração**

### **❌ Variáveis de Ambiente**

#### **Configurações não carregam**
1. **Backend - appsettings.json**:
   ```json
   {
     "Environment": "Development",
     "DetailedErrors": true,
     "Logging": {
       "LogLevel": {
         "Default": "Information"
       }
     }
   }
   ```

2. **Frontend - .env.local**:
   ```env
   VITE_API_URL=http://localhost:5268/api
   VITE_APP_NAME="Sistema ERP"
   ```

### **❌ SSL/HTTPS Issues**

#### **Certificate errors**
**Desenvolvimento**:
```bash
# Trust development certificate
dotnet dev-certs https --trust
```

**Produção**:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:5269"
      }
    }
  }
}
```

---

## 🐛 **Debug e Logs**

### **Habilitando Logs Detalhados**

#### **Backend Logging**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Warning",
      "GestaoRestaurante": "Debug"
    }
  }
}
```

#### **Frontend Debug**:
```typescript
// Habilitar debug no console
localStorage.setItem('debug', 'app:*');

// Logs customizados
console.log('Auth Store:', useAuthStore.getState());
```

### **SQL Profiling**:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## 🧪 **Testes e Validação**

### **Testando APIs manualmente**

#### **Health Check**:
```bash
curl -X GET http://localhost:5268/api/health
# Expected: {"status":"Healthy"}
```

#### **Login**:
```bash
curl -X POST http://localhost:5268/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@restaurantedemo.com","senha":"Admin123!"}'
```

#### **Protected Endpoint**:
```bash
curl -X GET http://localhost:5268/api/empresas \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### **Validando Database**:
```sql
-- Verificar tabelas essenciais
SELECT name FROM sys.tables WHERE name IN (
  'AspNetUsers', 'Empresas', 'Filiais', 
  'Agrupamentos', 'SubAgrupamentos', 'CentrosCusto',
  'Categorias', 'Produtos'
);

-- Verificar dados seed
SELECT COUNT(*) FROM AspNetUsers; -- Deve ter pelo menos 1
SELECT COUNT(*) FROM Empresas;    -- Deve ter pelo menos 1
```

---

## 📱 **Problemas Específicos por Browser**

### **Chrome Issues**
- **CORS**: Usar `--disable-web-security` flag (apenas dev)
- **Cache**: Hard refresh (Ctrl+Shift+R)
- **DevTools**: F12 → Console para erros

### **Firefox Issues**
- **Mixed Content**: about:config → security.mixed_content.block_active_content
- **Cache**: Ctrl+F5 para hard refresh

### **Edge Issues**
- Similar ao Chrome (baseado Chromium)
- Configurações de segurança podem bloquear localhost

---

## 🚨 **Procedimentos de Emergência**

### **Sistema Completamente Inoperante**

#### **Reset Completo - ÚLTIMO RECURSO**:
```bash
# 1. Parar todos os processos
pkill -f "dotnet"
pkill -f "node"

# 2. Reset database
cd backend/src
dotnet ef database drop --force --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
dotnet ef database update --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API

# 3. Rebuild backend
dotnet clean
dotnet restore
dotnet build

# 4. Rebuild frontend
cd ../../frontend
rm -rf node_modules package-lock.json
npm install
npm run build

# 5. Restart services
cd ../backend/src/GestaoRestaurante.API
dotnet run &
cd ../../../frontend
npm run dev
```

### **Backup de Emergência**
```sql
-- Backup rápido essencial
BACKUP DATABASE [GestaoRestaurante] 
TO DISK = 'C:\Temp\emergency_backup.bak'
WITH FORMAT, INIT;
```

---

## 📞 **Quando Entrar em Contato com Suporte**

### **Informações Obrigatórias**:
- ✅ Versão do sistema
- ✅ Ambiente (dev/prod)
- ✅ Browser e versão
- ✅ Sistema operacional
- ✅ Mensagem de erro exata
- ✅ Passos para reproduzir
- ✅ Logs relevantes

### **Logs para Anexar**:
```bash
# Backend logs
cd backend/src/GestaoRestaurante.API
dotnet run > logs.txt 2>&1

# Frontend logs (Console do browser)
F12 → Console → Copy all

# Database logs (Query execution plans)
SET STATISTICS IO ON;
-- Execute query problemática
```

---

**💡 Dica: Mantenha este guia acessível e sempre tente soluções simples primeiro!**

*Última atualização: Setembro 2024*
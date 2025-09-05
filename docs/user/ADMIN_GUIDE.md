# 🛡️ Manual do Administrador - Sistema ERP Restaurantes

Guia completo para administradores do Sistema ERP de Gestão de Restaurantes.

## 👨‍💼 **Introdução - Papel do Administrador**

Como administrador do sistema, você é responsável por:
- 🏢 Configurar estrutura organizacional completa
- 👥 Gerenciar usuários e permissões
- 🔧 Manter configurações do sistema
- 📊 Monitorar performance e saúde do sistema
- 🛡️ Garantir segurança e integridade dos dados
- 📈 Analisar métricas e relatórios

## 🚀 **Acesso Administrativo**

### **Credenciais de Administrador**
**Credenciais Padrão do Sistema**:
- Email: `admin@restaurantedemo.com`
- Senha: `Admin123!`
- Perfil: `ADMIN`
- Módulos: Acesso total a todos os módulos

### **Primeiro Acesso**
1. **Acesse**: http://localhost:5173
2. **Login**: Use as credenciais de administrador
3. **Alterar Senha**: Recomendado alterar a senha padrão
4. **Verificar Módulos**: Confirme acesso a todos os módulos

## 🏗️ **Configuração Inicial do Sistema**

### **1. Estrutura Organizacional**

#### **Ordem de Configuração**:
```
1️⃣ Empresa (Matriz)
2️⃣ Filiais
3️⃣ Agrupamentos por Filial
4️⃣ Sub Agrupamentos
5️⃣ Centros de Custo
6️⃣ Categorias (3 níveis)
7️⃣ Produtos
```

#### **Configuração da Empresa Principal**:
```json
{
  "razaoSocial": "Restaurante Matriz Ltda",
  "nomeFantasia": "Restaurante Matriz",
  "cnpj": "12.345.678/0001-90",
  "email": "contato@restaurantematriz.com",
  "telefone": "(11) 3333-4444",
  "endereco": {
    "cep": "01234-567",
    "logradouro": "Rua Principal, 100",
    "bairro": "Centro",
    "cidade": "São Paulo", 
    "estado": "SP"
  }
}
```

### **2. Estrutura de Filiais**

#### **Exemplo de Filiais**:
```
🏢 Restaurante Matriz Ltda
├── 🏪 Filial Centro (CNPJ: /0002-71)
├── 🏪 Filial Shopping (CNPJ: /0003-52) 
└── 🏪 Filial Delivery (CNPJ: /0004-33)
```

### **3. Estrutura de Centro de Custos**

#### **Exemplo Completo por Filial**:
```
🏪 Filial Centro
├── 📊 Cozinha
│   ├── 📋 Preparação
│   │   ├── 💰 Fogão Industrial
│   │   ├── 💰 Bancada Principal
│   │   └── 💰 Fritadeira
│   └── 📋 Limpeza
│       ├── 💰 Pia Industrial
│       └── 💰 Máquina Lavar
├── 📊 Salão
│   ├── 📋 Atendimento
│   │   ├── 💰 Mesa 01
│   │   ├── 💰 Mesa 02
│   │   └── 💰 Balcão
│   └── 📋 Caixa
│       └── 💰 Terminal PDV
└── 📊 Administrativo
    └── 📋 Escritório
        ├── 💰 Computador Gerente
        └── 💰 Impressora
```

### **4. Estrutura de Categorias**

#### **Exemplo Hierárquico Completo**:
```
📂 Bebidas (BEB - Nível 1)
├── 🥤 Refrigerantes (BEB.REF - Nível 2)
│   ├── Cola (BEB.REF.COL - Nível 3)
│   ├── Limão (BEB.REF.LIM - Nível 3)
│   └── Laranja (BEB.REF.LAR - Nível 3)
├── 🍺 Cervejas (BEB.CER - Nível 2)
│   ├── Pilsen (BEB.CER.PIL - Nível 3)
│   └── IPA (BEB.CER.IPA - Nível 3)
└── ☕ Café (BEB.CAF - Nível 2)
    ├── Expresso (BEB.CAF.EXP - Nível 3)
    └── Cappuccino (BEB.CAF.CAP - Nível 3)

🍔 Lanches (LAN - Nível 1)
├── 🍔 Hambúrgueres (LAN.HAM - Nível 2)
│   ├── Tradicional (LAN.HAM.TRA - Nível 3)
│   └── Gourmet (LAN.HAM.GOU - Nível 3)
└── 🌭 Hot Dogs (LAN.HOT - Nível 2)
    ├── Simples (LAN.HOT.SIM - Nível 3)
    └── Especial (LAN.HOT.ESP - Nível 3)
```

## 👥 **Gerenciamento de Usuários**

### **Perfis do Sistema**

#### **ADMIN (Administrador)**
```json
{
  "perfil": "ADMIN",
  "modulosLiberados": [
    "EMPRESAS", "FILIAIS", "CENTRO_CUSTO",
    "CATEGORIAS", "PRODUTOS", "USUARIOS",
    "RELATORIOS", "CONFIGURACOES"
  ],
  "permissoes": {
    "create": true,
    "read": true, 
    "update": true,
    "delete": true,
    "admin": true
  }
}
```

#### **GERENTE (Gerente)**
```json
{
  "perfil": "GERENTE", 
  "modulosLiberados": [
    "EMPRESAS", "FILIAIS", "CENTRO_CUSTO",
    "CATEGORIAS", "PRODUTOS", "RELATORIOS"
  ],
  "permissoes": {
    "create": true,
    "read": true,
    "update": true, 
    "delete": false,
    "admin": false
  }
}
```

#### **OPERADOR (Operador)**
```json
{
  "perfil": "OPERADOR",
  "modulosLiberados": [
    "PRODUTOS", "CATEGORIAS"
  ],
  "permissoes": {
    "create": true,
    "read": true,
    "update": true,
    "delete": false,
    "admin": false
  }
}
```

#### **USUARIO (Usuário Básico)**
```json
{
  "perfil": "USUARIO",
  "modulosLiberados": [
    "PRODUTOS"
  ],
  "permissoes": {
    "create": false,
    "read": true,
    "update": false,
    "delete": false,
    "admin": false
  }
}
```

### **Criando Usuários**

#### **Dados Obrigatórios**:
```json
{
  "nome": "João Silva",
  "email": "joao.silva@restaurante.com",
  "senha": "SenhaSegura123!",
  "confirmarSenha": "SenhaSegura123!",
  "empresaId": "guid-da-empresa",
  "perfil": "GERENTE",
  "filiaisAcesso": ["guid-filial-1", "guid-filial-2"]
}
```

#### **Política de Senhas**:
- ✅ Mínimo 8 caracteres
- ✅ Pelo menos 1 letra maiúscula
- ✅ Pelo menos 1 número
- ✅ Pelo menos 1 caractere especial
- ❌ Não pode conter nome ou email
- ❌ Não pode ser senha comum (123456, password, etc.)

## 📊 **Monitoramento e Métricas**

### **Health Checks**

#### **Endpoints de Monitoramento**:
```bash
# Health Check Básico
GET http://localhost:5268/api/health
Response: {"status": "Healthy", "timestamp": "2024-09-04T12:00:00Z"}

# Health Check Detalhado  
GET http://localhost:5268/api/health/detailed
Response: {
  "status": "Healthy",
  "checks": {
    "database": {"status": "Healthy", "responseTime": "15ms"},
    "cache": {"status": "Healthy", "responseTime": "2ms"},
    "system": {"status": "Healthy", "memoryUsage": "45%"}
  }
}

# Liveness Probe
GET http://localhost:5268/api/health/live

# Readiness Probe
GET http://localhost:5268/api/health/ready
```

### **Métricas de Performance**

#### **Métricas Básicas**:
```bash
GET http://localhost:5268/api/metrics
```

**Response**:
```json
{
  "totalRequests": 15420,
  "averageResponseTime": "125ms", 
  "errorRate": "0.2%",
  "uptime": "15d 4h 32m",
  "activeUsers": 23,
  "peakUsers": 45,
  "databaseConnections": 12
}
```

#### **Métricas Detalhadas**:
```bash
GET http://localhost:5268/api/metrics/performance
```

**Response**:
```json
{
  "requests": {
    "total": 15420,
    "perSecond": 12.5,
    "byEndpoint": {
      "/api/empresas": 3240,
      "/api/produtos": 5670,
      "/api/auth/login": 890
    }
  },
  "performance": {
    "averageResponseTime": "125ms",
    "p95ResponseTime": "450ms",
    "slowestEndpoints": [
      {"endpoint": "/api/produtos", "averageTime": "280ms"}
    ]
  },
  "errors": {
    "totalErrors": 45,
    "errorRate": "0.2%",
    "errorsByType": {
      "400": 25,
      "401": 15, 
      "500": 5
    }
  }
}
```

### **Alertas e Monitoramento**

#### **Thresholds de Alerta**:
```json
{
  "responseTime": {
    "warning": "500ms",
    "critical": "1000ms"
  },
  "errorRate": {
    "warning": "1%",
    "critical": "5%" 
  },
  "memoryUsage": {
    "warning": "70%",
    "critical": "85%"
  },
  "cpuUsage": {
    "warning": "80%", 
    "critical": "95%"
  },
  "diskSpace": {
    "warning": "80%",
    "critical": "90%"
  }
}
```

## 🗄️ **Administração do Banco de Dados**

### **Informações da Base**
- **Servidor**: SQL Server
- **Database**: `GestaoRestaurante`
- **Connection String**: Ver `appsettings.json`

### **Tabelas Principais**
```sql
-- Estrutura organizacional
Empresas (25 registros)
Filiais (8 registros)  
Agrupamentos (15 registros)
SubAgrupamentos (45 registros)
CentrosCusto (120 registros)

-- Produtos e categorias
Categorias (85 registros)
Produtos (340 registros)
Ingredientes (120 registros)

-- Usuários e segurança
AspNetUsers (12 usuários)
AspNetRoles (4 perfis)
PlanoAssinatura (3 planos)
```

### **Backup e Manutenção**

#### **Backup Diário**:
```sql
-- Script de backup
BACKUP DATABASE [GestaoRestaurante] 
TO DISK = 'C:\Backups\GestaoRestaurante_' + 
          FORMAT(GETDATE(), 'yyyyMMdd_HHmmss') + '.bak'
WITH FORMAT, INIT, NAME = 'GestaoRestaurante-Full Database Backup';
```

#### **Limpeza de Logs**:
```sql
-- Limpar logs de operação antigos (> 90 dias)
DELETE FROM LogOperacoes 
WHERE DataOperacao < DATEADD(DAY, -90, GETDATE());
```

#### **Otimização de Performance**:
```sql
-- Rebuild indexes semanalmente
ALTER INDEX ALL ON Produtos REBUILD;
ALTER INDEX ALL ON Empresas REBUILD;

-- Update statistics
UPDATE STATISTICS Produtos;
UPDATE STATISTICS Empresas;
```

### **Migrations**

#### **Aplicar Migrations**:
```bash
# Verificar pending migrations
dotnet ef migrations list --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API

# Aplicar migrations
dotnet ef database update --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
```

#### **Criar Nova Migration**:
```bash
dotnet ef migrations add NomeDaMigration --project GestaoRestaurante.Infrastructure --startup-project GestaoRestaurante.API
```

## 🔧 **Configurações do Sistema**

### **appsettings.json (Produção)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=GestaoRestaurante;Integrated Security=true;TrustServerCertificate=true"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-super-secreta-producao-256-bits", 
    "Issuer": "GestaoRestauranteAPI",
    "Audience": "GestaoRestauranteFrontend", 
    "ExpirationInHours": 8
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "GestaoRestaurante": "Information"
    }
  },
  "RateLimiting": {
    "GlobalLimit": 100,
    "AuthLimit": 10,
    "WindowInMinutes": 1
  }
}
```

### **Configurações de Cache**
```json
{
  "CacheSettings": {
    "DefaultExpirationMinutes": 30,
    "EmpresasExpirationMinutes": 60,
    "ProdutosExpirationMinutes": 15,
    "UsuariosExpirationMinutes": 10
  }
}
```

## 🛡️ **Segurança e Auditoria**

### **Auditoria de Operações**

#### **Log de Operações**:
Todas as operações são registradas na tabela `LogOperacoes`:
```sql
SELECT 
  lo.DataOperacao,
  lo.Usuario,
  lo.Acao,
  lo.Entidade,
  lo.EntidadeId,
  lo.DadosAnteriores,
  lo.DadosNovos
FROM LogOperacoes lo
WHERE lo.DataOperacao >= DATEADD(DAY, -7, GETDATE())
ORDER BY lo.DataOperacao DESC;
```

#### **Operações Críticas Monitoradas**:
- ✅ Login/Logout de usuários
- ✅ Criação/alteração de empresas
- ✅ Mudanças em usuários e permissões
- ✅ Exclusão de registros importantes
- ✅ Alterações em configurações

### **Políticas de Segurança**

#### **Sessão e Tokens**:
- **Expiração Token JWT**: 8 horas
- **Refresh Token**: Não implementado (futuro)
- **Logout Automático**: Por inatividade
- **Rate Limiting**: 100 req/min por IP

#### **Controle de Acesso**:
- **Multi-tenancy**: Por empresa
- **Autorização Granular**: Por módulo
- **Filiais**: Controle por usuário
- **Auditoria Completa**: Todas operações

### **Backup de Segurança**

#### **Estratégia 3-2-1**:
- **3 cópias** dos dados
- **2 mídias diferentes** (local + nuvem)
- **1 cópia off-site** (backup remoto)

#### **Frequência**:
- **Diário**: Backup incremental
- **Semanal**: Backup completo
- **Mensal**: Backup para arquivo
- **Teste Restore**: Mensal

## 📈 **Relatórios Administrativos**

### **Relatórios de Uso**

#### **Usuários Ativos**:
```sql
SELECT 
  u.UserName,
  u.Email,
  COUNT(lo.Id) as TotalOperacoes,
  MAX(lo.DataOperacao) as UltimoAcesso
FROM AspNetUsers u
LEFT JOIN LogOperacoes lo ON u.Id = lo.UsuarioId
WHERE u.LockoutEnd IS NULL OR u.LockoutEnd < GETDATE()
GROUP BY u.UserName, u.Email
ORDER BY UltimoAcesso DESC;
```

#### **Módulos Mais Utilizados**:
```sql
SELECT 
  lo.Entidade,
  COUNT(*) as TotalOperacoes,
  COUNT(DISTINCT lo.UsuarioId) as UsuariosUnicos
FROM LogOperacoes lo
WHERE lo.DataOperacao >= DATEADD(DAY, -30, GETDATE())
GROUP BY lo.Entidade
ORDER BY TotalOperacoes DESC;
```

### **Relatórios de Performance**

#### **Endpoints Mais Lentos**:
```sql
SELECT 
  Endpoint,
  AVG(TempoResposta) as TempoMedio,
  MAX(TempoResposta) as TempoMaximo,
  COUNT(*) as TotalRequests
FROM PerformanceLogs
WHERE DataRequest >= DATEADD(DAY, -7, GETDATE())
GROUP BY Endpoint
ORDER BY TempoMedio DESC;
```

## 🚨 **Troubleshooting Administrativo**

### **Problemas Comuns**

#### **Alta Utilização de CPU**
```bash
# Verificar processos
SELECT 
  session_id,
  cpu_time,
  memory_usage,
  last_request_start_time,
  status
FROM sys.dm_exec_sessions
WHERE is_user_process = 1
ORDER BY cpu_time DESC;
```

#### **Conexões de Database**
```sql
SELECT 
  DB_NAME(database_id) as DatabaseName,
  COUNT(session_id) as ConnectionCount,
  login_name,
  host_name,
  program_name
FROM sys.dm_exec_sessions
WHERE database_id > 0
GROUP BY database_id, login_name, host_name, program_name
ORDER BY ConnectionCount DESC;
```

#### **Logs de Erro**
```sql
-- Erros nas últimas 24 horas
SELECT 
  DataOperacao,
  Usuario, 
  Acao,
  Entidade,
  MensagemErro
FROM LogOperacoes
WHERE TipoOperacao = 'ERRO'
  AND DataOperacao >= DATEADD(HOUR, -24, GETDATE())
ORDER BY DataOperacao DESC;
```

### **Performance Tuning**

#### **Otimizações Recomendadas**:
- ✅ **Índices**: Revisar e otimizar monthly
- ✅ **Queries**: Analisar planos de execução
- ✅ **Cache**: Configurar TTL apropriado
- ✅ **Connection Pool**: Monitorar utilização
- ✅ **Memory**: Monitorar vazamentos

## 🔄 **Manutenção Preventiva**

### **Checklist Diário**
- [ ] Verificar health checks
- [ ] Revisar logs de erro
- [ ] Monitorar performance
- [ ] Verificar espaço em disco
- [ ] Confirmar backups

### **Checklist Semanal**  
- [ ] Analisar métricas de uso
- [ ] Otimizar índices database
- [ ] Revisar usuários inativos
- [ ] Testar procedimentos de restore
- [ ] Atualizar documentação

### **Checklist Mensal**
- [ ] Relatório completo de performance
- [ ] Auditoria de segurança
- [ ] Limpeza de logs antigos
- [ ] Revisão de permissões
- [ ] Planejamento de capacidade

---

**Sistema robusto requer administração proativa. Use este guia para manter tudo funcionando perfeitamente!**
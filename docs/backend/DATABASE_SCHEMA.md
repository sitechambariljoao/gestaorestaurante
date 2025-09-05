# 🗄️ Database Schema - Sistema ERP Restaurantes

Documentação completa do esquema de banco de dados SQL Server.

## 📊 **Visão Geral da Database**

### **Informações Gerais**
- **Database**: `GestaoRestaurante`
- **Engine**: SQL Server 2019+
- **Collation**: `SQL_Latin1_General_CP1_CI_AS`
- **Total Tables**: 25+
- **Total Records**: ~500+ (com seed data)

### **Organização por Módulos**
```
🎯 Core (8 tables)        - Sistema base, autenticação
🏢 Empresas (1 table)     - Gestão empresarial  
🏪 Filiais (2 tables)     - Gestão filiais + vínculos
📊 CentroCusto (4 tables) - Estrutura hierárquica
📂 Categorias (1 table)   - Hierarquia produtos
🛍️ Produtos (3 tables)   - Produtos + ingredientes
🍽️ Cardapio (1 table)    - Estrutura cardápio
📋 Pedidos (2 tables)     - Sistema pedidos
👥 Funcionarios (2 tables) - RH e jornada
💰 Financeiro (1 table)   - Movimentações
📦 Estoque (1 table)      - Controle estoque
```

---

## 🎯 **Módulo Core - Sistema Base**

### **AspNetUsers** (Usuários do sistema)
```sql
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET(7) NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    
    -- Campos customizados
    [Nome] NVARCHAR(100) NOT NULL,
    [Cpf] NVARCHAR(18) NULL,
    [Perfil] NVARCHAR(50) NOT NULL DEFAULT 'USUARIO',
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    [EmpresaId] UNIQUEIDENTIFIER NOT NULL,
    
    CONSTRAINT [FK_AspNetUsers_Empresas] 
        FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas]([Id])
);

-- Indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_Email] 
    ON [AspNetUsers] ([NormalizedEmail]) WHERE [NormalizedEmail] IS NOT NULL;
CREATE NONCLUSTERED INDEX [IX_AspNetUsers_EmpresaId] 
    ON [AspNetUsers] ([EmpresaId]);
```

### **AspNetRoles** (Perfis de acesso)
```sql
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL
);

-- Dados iniciais
INSERT INTO [AspNetRoles] VALUES
('1', 'ADMIN', 'ADMIN', NULL),
('2', 'GERENTE', 'GERENTE', NULL), 
('3', 'OPERADOR', 'OPERADOR', NULL),
('4', 'USUARIO', 'USUARIO', NULL);
```

### **PlanoAssinatura** (Planos do sistema)
```sql
CREATE TABLE [dbo].[PlanoAssinatura] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(500) NULL,
    [Valor] DECIMAL(10,2) NOT NULL,
    [PeriodoMeses] INT NOT NULL,
    [LimiteUsuarios] INT NOT NULL,
    [LimiteFiliais] INT NOT NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Dados iniciais
INSERT INTO [PlanoAssinatura] VALUES
(NEWID(), 'Básico', 'Plano básico para pequenos restaurantes', 99.00, 1, 5, 2, 1, GETDATE()),
(NEWID(), 'Profissional', 'Plano avançado para redes médias', 199.00, 1, 20, 10, 1, GETDATE()),
(NEWID(), 'Enterprise', 'Plano completo para grandes redes', 399.00, 1, 100, 50, 1, GETDATE());
```

### **ModuloPlano** (Módulos liberados por plano)
```sql
CREATE TABLE [dbo].[ModuloPlano] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [PlanoAssinaturaId] UNIQUEIDENTIFIER NOT NULL,
    [NomeModulo] NVARCHAR(50) NOT NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [FK_ModuloPlano_PlanoAssinatura] 
        FOREIGN KEY ([PlanoAssinaturaId]) REFERENCES [PlanoAssinatura]([Id])
);
```

### **AssinaturaEmpresa** (Assinatura ativa da empresa)
```sql
CREATE TABLE [dbo].[AssinaturaEmpresa] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EmpresaId] UNIQUEIDENTIFIER NOT NULL,
    [PlanoAssinaturaId] UNIQUEIDENTIFIER NOT NULL,
    [DataInicio] DATETIME2 NOT NULL,
    [DataVencimento] DATETIME2 NOT NULL,
    [Ativa] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_AssinaturaEmpresa_Empresa] 
        FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas]([Id]),
    CONSTRAINT [FK_AssinaturaEmpresa_PlanoAssinatura] 
        FOREIGN KEY ([PlanoAssinaturaId]) REFERENCES [PlanoAssinatura]([Id])
);
```

### **LogOperacao** (Auditoria do sistema)
```sql
CREATE TABLE [dbo].[LogOperacao] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UsuarioId] NVARCHAR(450) NULL,
    [Usuario] NVARCHAR(256) NOT NULL,
    [Acao] NVARCHAR(50) NOT NULL,
    [Entidade] NVARCHAR(100) NOT NULL,
    [EntidadeId] NVARCHAR(450) NULL,
    [DadosAnteriores] NVARCHAR(MAX) NULL,
    [DadosNovos] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [DataOperacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_LogOperacao_AspNetUsers] 
        FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers]([Id])
);

-- Indexes para performance
CREATE NONCLUSTERED INDEX [IX_LogOperacao_DataOperacao] 
    ON [LogOperacao] ([DataOperacao] DESC);
CREATE NONCLUSTERED INDEX [IX_LogOperacao_Usuario_Entidade] 
    ON [LogOperacao] ([Usuario], [Entidade]);
```

---

## 🏢 **Módulo Empresas**

### **Empresas** (Empresas do sistema)
```sql
CREATE TABLE [dbo].[Empresas] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [RazaoSocial] NVARCHAR(255) NOT NULL,
    [NomeFantasia] NVARCHAR(255) NOT NULL,
    [Cnpj] NVARCHAR(18) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Telefone] NVARCHAR(20) NULL,
    [Ativa] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    -- Endereço (Value Object inline)
    [Endereco_Cep] NVARCHAR(10) NULL,
    [Endereco_Logradouro] NVARCHAR(255) NULL,
    [Endereco_Numero] NVARCHAR(10) NULL,
    [Endereco_Complemento] NVARCHAR(100) NULL,
    [Endereco_Bairro] NVARCHAR(100) NULL,
    [Endereco_Cidade] NVARCHAR(100) NULL,
    [Endereco_Estado] NVARCHAR(2) NULL
);

-- Constraints e Indexes
ALTER TABLE [Empresas] 
    ADD CONSTRAINT [UK_Empresas_Cnpj] UNIQUE ([Cnpj]);
ALTER TABLE [Empresas] 
    ADD CONSTRAINT [UK_Empresas_Email] UNIQUE ([Email]);
CREATE NONCLUSTERED INDEX [IX_Empresas_Ativa] 
    ON [Empresas] ([Ativa]);
```

---

## 🏪 **Módulo Filiais**

### **Filiais** (Filiais das empresas)
```sql
CREATE TABLE [dbo].[Filiais] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EmpresaId] UNIQUEIDENTIFIER NOT NULL,
    [RazaoSocial] NVARCHAR(255) NOT NULL,
    [NomeFantasia] NVARCHAR(255) NOT NULL,
    [Cnpj] NVARCHAR(18) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Telefone] NVARCHAR(20) NULL,
    [Ativa] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    -- Endereço (Value Object inline)
    [Endereco_Cep] NVARCHAR(10) NULL,
    [Endereco_Logradouro] NVARCHAR(255) NULL,
    [Endereco_Numero] NVARCHAR(10) NULL,
    [Endereco_Complemento] NVARCHAR(100) NULL,
    [Endereco_Bairro] NVARCHAR(100) NULL,
    [Endereco_Cidade] NVARCHAR(100) NULL,
    [Endereco_Estado] NVARCHAR(2) NULL,
    
    CONSTRAINT [FK_Filiais_Empresas] 
        FOREIGN KEY ([EmpresaId]) REFERENCES [Empresas]([Id])
);

-- Constraints e Indexes
ALTER TABLE [Filiais] 
    ADD CONSTRAINT [UK_Filiais_Cnpj] UNIQUE ([Cnpj]);
ALTER TABLE [Filiais] 
    ADD CONSTRAINT [UK_Filiais_Email] UNIQUE ([Email]);
CREATE NONCLUSTERED INDEX [IX_Filiais_EmpresaId] 
    ON [Filiais] ([EmpresaId]);
```

### **UsuarioFilial** (Acesso usuários por filial)
```sql
CREATE TABLE [dbo].[UsuarioFilial] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UsuarioId] NVARCHAR(450) NOT NULL,
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_UsuarioFilial_AspNetUsers] 
        FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_UsuarioFilial_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id])
);

-- Constraint única
ALTER TABLE [UsuarioFilial] 
    ADD CONSTRAINT [UK_UsuarioFilial] UNIQUE ([UsuarioId], [FilialId]);
```

---

## 📊 **Módulo Centro de Custos**

### **Agrupamentos** (Nível 1 da hierarquia)
```sql
CREATE TABLE [dbo].[Agrupamentos] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(500) NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    CONSTRAINT [FK_Agrupamentos_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id])
);

-- Constraints compostas (único por filial)
ALTER TABLE [Agrupamentos] 
    ADD CONSTRAINT [UK_Agrupamentos_Codigo_Filial] 
    UNIQUE ([Codigo], [FilialId]);
ALTER TABLE [Agrupamentos] 
    ADD CONSTRAINT [UK_Agrupamentos_Nome_Filial] 
    UNIQUE ([Nome], [FilialId]);
```

### **SubAgrupamentos** (Nível 2 da hierarquia)
```sql
CREATE TABLE [dbo].[SubAgrupamentos] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [AgrupamentoId] UNIQUEIDENTIFIER NOT NULL,
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(500) NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    CONSTRAINT [FK_SubAgrupamentos_Agrupamentos] 
        FOREIGN KEY ([AgrupamentoId]) REFERENCES [Agrupamentos]([Id])
);

-- Constraints compostas (único por agrupamento)
ALTER TABLE [SubAgrupamentos] 
    ADD CONSTRAINT [UK_SubAgrupamentos_Codigo_Agrupamento] 
    UNIQUE ([Codigo], [AgrupamentoId]);
ALTER TABLE [SubAgrupamentos] 
    ADD CONSTRAINT [UK_SubAgrupamentos_Nome_Agrupamento] 
    UNIQUE ([Nome], [AgrupamentoId]);
```

### **CentrosCusto** (Nível 3 da hierarquia)
```sql
CREATE TABLE [dbo].[CentrosCusto] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [SubAgrupamentoId] UNIQUEIDENTIFIER NOT NULL,
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(500) NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    CONSTRAINT [FK_CentrosCusto_SubAgrupamentos] 
        FOREIGN KEY ([SubAgrupamentoId]) REFERENCES [SubAgrupamentos]([Id])
);

-- Constraints compostas (único por sub-agrupamento)
ALTER TABLE [CentrosCusto] 
    ADD CONSTRAINT [UK_CentrosCusto_Codigo_SubAgrupamento] 
    UNIQUE ([Codigo], [SubAgrupamentoId]);
ALTER TABLE [CentrosCusto] 
    ADD CONSTRAINT [UK_CentrosCusto_Nome_SubAgrupamento] 
    UNIQUE ([Nome], [SubAgrupamentoId]);
```

### **FilialAgrupamento** (Vínculo filiais-agrupamentos)
```sql
CREATE TABLE [dbo].[FilialAgrupamento] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [AgrupamentoId] UNIQUEIDENTIFIER NOT NULL,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_FilialAgrupamento_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id]),
    CONSTRAINT [FK_FilialAgrupamento_Agrupamentos] 
        FOREIGN KEY ([AgrupamentoId]) REFERENCES [Agrupamentos]([Id])
);

-- Constraint única
ALTER TABLE [FilialAgrupamento] 
    ADD CONSTRAINT [UK_FilialAgrupamento] UNIQUE ([FilialId], [AgrupamentoId]);
```

---

## 📂 **Módulo Categorias**

### **Categorias** (Hierarquia de produtos - 3 níveis)
```sql
CREATE TABLE [dbo].[Categorias] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [CentroCustoId] UNIQUEIDENTIFIER NOT NULL,
    [CategoriaPaiId] UNIQUEIDENTIFIER NULL, -- Para subcategorias
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(500) NULL,
    [Nivel] INT NOT NULL DEFAULT 1, -- 1, 2, ou 3
    [Ativa] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    CONSTRAINT [FK_Categorias_CentrosCusto] 
        FOREIGN KEY ([CentroCustoId]) REFERENCES [CentrosCusto]([Id]),
    CONSTRAINT [FK_Categorias_CategoriaPai] 
        FOREIGN KEY ([CategoriaPaiId]) REFERENCES [Categorias]([Id])
);

-- Constraints e Indexes
ALTER TABLE [Categorias] 
    ADD CONSTRAINT [UK_Categorias_Codigo_CentroCusto] 
    UNIQUE ([Codigo], [CentroCustoId]);
ALTER TABLE [Categorias] 
    ADD CONSTRAINT [UK_Categorias_Nome_CentroCusto] 
    UNIQUE ([Nome], [CentroCustoId]);
ALTER TABLE [Categorias] 
    ADD CONSTRAINT [CK_Categorias_Nivel] CHECK ([Nivel] BETWEEN 1 AND 3);
CREATE NONCLUSTERED INDEX [IX_Categorias_CategoriaPai] 
    ON [Categorias] ([CategoriaPaiId]);
```

---

## 🛍️ **Módulo Produtos**

### **Produtos** (Produtos do cardápio)
```sql
CREATE TABLE [dbo].[Produtos] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [CategoriaId] UNIQUEIDENTIFIER NOT NULL,
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Descricao] NVARCHAR(1000) NULL,
    [UnidadeMedida] NVARCHAR(10) NOT NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataUltimaAlteracao] DATETIME2 NULL,
    
    CONSTRAINT [FK_Produtos_Categorias] 
        FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias]([Id])
);

-- Constraints
ALTER TABLE [Produtos] 
    ADD CONSTRAINT [UK_Produtos_Codigo] UNIQUE ([Codigo]); -- Global unique
ALTER TABLE [Produtos] 
    ADD CONSTRAINT [UK_Produtos_Nome_Categoria] 
    UNIQUE ([Nome], [CategoriaId]);
CREATE NONCLUSTERED INDEX [IX_Produtos_CategoriaId] 
    ON [Produtos] ([CategoriaId]);
CREATE NONCLUSTERED INDEX [IX_Produtos_Nome] 
    ON [Produtos] ([Nome]); -- Para busca
```

### **Ingredientes** (Ingredientes base)
```sql
CREATE TABLE [dbo].[Ingredientes] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Nome] NVARCHAR(100) NOT NULL,
    [UnidadeMedida] NVARCHAR(10) NOT NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Constraint
ALTER TABLE [Ingredientes] 
    ADD CONSTRAINT [UK_Ingredientes_Nome] UNIQUE ([Nome]);
```

### **ProdutoIngrediente** (Receita dos produtos)
```sql
CREATE TABLE [dbo].[ProdutoIngrediente] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ProdutoId] UNIQUEIDENTIFIER NOT NULL,
    [IngredienteId] UNIQUEIDENTIFIER NOT NULL,
    [Quantidade] DECIMAL(10,3) NOT NULL,
    [UnidadeMedida] NVARCHAR(10) NOT NULL,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_ProdutoIngrediente_Produtos] 
        FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos]([Id]),
    CONSTRAINT [FK_ProdutoIngrediente_Ingredientes] 
        FOREIGN KEY ([IngredienteId]) REFERENCES [Ingredientes]([Id])
);

-- Constraint única
ALTER TABLE [ProdutoIngrediente] 
    ADD CONSTRAINT [UK_ProdutoIngrediente] UNIQUE ([ProdutoId], [IngredienteId]);
```

---

## 🍽️ **Módulo Cardápio** (Futuro)

### **Mesas** (Mesas do restaurante)
```sql
CREATE TABLE [dbo].[Mesas] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [Numero] NVARCHAR(10) NOT NULL,
    [Capacidade] INT NOT NULL,
    [Localizacao] NVARCHAR(100) NULL,
    [Ativa] BIT NOT NULL DEFAULT 1,
    [DataCriacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [FK_Mesas_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id])
);

-- Constraint única por filial
ALTER TABLE [Mesas] 
    ADD CONSTRAINT [UK_Mesas_Numero_Filial] UNIQUE ([Numero], [FilialId]);
```

---

## 📋 **Módulo Pedidos** (Futuro)

### **Pedidos** (Pedidos dos clientes)
```sql
CREATE TABLE [dbo].[Pedidos] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [MesaId] UNIQUEIDENTIFIER NULL,
    [NumeroPedido] NVARCHAR(20) NOT NULL,
    [ClienteNome] NVARCHAR(100) NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'ABERTO',
    [DataPedido] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [DataFinalizacao] DATETIME2 NULL,
    [ValorTotal] DECIMAL(10,2) NOT NULL DEFAULT 0,
    [Observacoes] NVARCHAR(500) NULL,
    
    CONSTRAINT [FK_Pedidos_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id]),
    CONSTRAINT [FK_Pedidos_Mesas] 
        FOREIGN KEY ([MesaId]) REFERENCES [Mesas]([Id])
);
```

### **ItensPedido** (Itens do pedido)
```sql
CREATE TABLE [dbo].[ItensPedido] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [PedidoId] UNIQUEIDENTIFIER NOT NULL,
    [ProdutoId] UNIQUEIDENTIFIER NOT NULL,
    [Quantidade] DECIMAL(10,2) NOT NULL,
    [PrecoUnitario] DECIMAL(10,2) NOT NULL,
    [ValorTotal] DECIMAL(10,2) NOT NULL,
    [Observacoes] NVARCHAR(200) NULL,
    
    CONSTRAINT [FK_ItensPedido_Pedidos] 
        FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos]([Id]),
    CONSTRAINT [FK_ItensPedido_Produtos] 
        FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos]([Id])
);
```

---

## 👥 **Módulo Funcionários** (Futuro)

### **Funcionarios**
```sql
CREATE TABLE [dbo].[Funcionarios] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Cpf] NVARCHAR(18) NOT NULL,
    [Email] NVARCHAR(255) NULL,
    [Telefone] NVARCHAR(20) NULL,
    [Cargo] NVARCHAR(50) NOT NULL,
    [Salario] DECIMAL(10,2) NULL,
    [DataAdmissao] DATETIME2 NOT NULL,
    [DataDemissao] DATETIME2 NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [FK_Funcionarios_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id])
);
```

### **RegistroJornada**
```sql
CREATE TABLE [dbo].[RegistroJornada] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FuncionarioId] UNIQUEIDENTIFIER NOT NULL,
    [DataRegistro] DATE NOT NULL,
    [HoraEntrada] TIME NULL,
    [HoraSaida] TIME NULL,
    [HorasTrabalhadasDecimal] DECIMAL(4,2) NULL,
    [TipoRegistro] NVARCHAR(20) NOT NULL DEFAULT 'NORMAL',
    
    CONSTRAINT [FK_RegistroJornada_Funcionarios] 
        FOREIGN KEY ([FuncionarioId]) REFERENCES [Funcionarios]([Id])
);
```

---

## 💰 **Módulo Financeiro** (Futuro)

### **MovimentacaoFinanceira**
```sql
CREATE TABLE [dbo].[MovimentacaoFinanceira] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [TipoMovimentacao] NVARCHAR(20) NOT NULL, -- ENTRADA, SAIDA
    [Categoria] NVARCHAR(50) NOT NULL,
    [Descricao] NVARCHAR(200) NOT NULL,
    [Valor] DECIMAL(10,2) NOT NULL,
    [DataMovimentacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [FormaPagamento] NVARCHAR(30) NULL,
    [Observacoes] NVARCHAR(500) NULL,
    
    CONSTRAINT [FK_MovimentacaoFinanceira_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id])
);
```

---

## 📦 **Módulo Estoque** (Futuro)

### **MovimentacaoEstoque**
```sql
CREATE TABLE [dbo].[MovimentacaoEstoque] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [FilialId] UNIQUEIDENTIFIER NOT NULL,
    [IngredienteId] UNIQUEIDENTIFIER NOT NULL,
    [TipoMovimentacao] NVARCHAR(20) NOT NULL, -- ENTRADA, SAIDA, AJUSTE
    [Quantidade] DECIMAL(10,3) NOT NULL,
    [ValorUnitario] DECIMAL(10,2) NULL,
    [ValorTotal] DECIMAL(10,2) NULL,
    [DataMovimentacao] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Motivo] NVARCHAR(100) NULL,
    [DocumentoReferencia] NVARCHAR(50) NULL,
    
    CONSTRAINT [FK_MovimentacaoEstoque_Filiais] 
        FOREIGN KEY ([FilialId]) REFERENCES [Filiais]([Id]),
    CONSTRAINT [FK_MovimentacaoEstoque_Ingredientes] 
        FOREIGN KEY ([IngredienteId]) REFERENCES [Ingredientes]([Id])
);
```

---

## 📊 **Views e Stored Procedures**

### **View - Estrutura Hierárquica Completa**
```sql
CREATE VIEW [dbo].[vw_EstruturaCompleta] AS
SELECT 
    e.Id as EmpresaId,
    e.NomeFantasia as EmpresaNome,
    f.Id as FilialId,  
    f.NomeFantasia as FilialNome,
    a.Id as AgrupamentoId,
    a.Nome as AgrupamentoNome,
    sa.Id as SubAgrupamentoId,
    sa.Nome as SubAgrupamentoNome,
    cc.Id as CentroCustoId,
    cc.Nome as CentroCustoNome,
    c.Id as CategoriaId,
    c.Nome as CategoriaNome,
    c.Nivel as CategoriaNivel,
    p.Id as ProdutoId,
    p.Nome as ProdutoNome
FROM Empresas e
    INNER JOIN Filiais f ON e.Id = f.EmpresaId
    INNER JOIN Agrupamentos a ON f.Id = a.FilialId  
    INNER JOIN SubAgrupamentos sa ON a.Id = sa.AgrupamentoId
    INNER JOIN CentrosCusto cc ON sa.Id = cc.SubAgrupamentoId
    LEFT JOIN Categorias c ON cc.Id = c.CentroCustoId
    LEFT JOIN Produtos p ON c.Id = p.CategoriaId
WHERE e.Ativa = 1 AND f.Ativa = 1 AND a.Ativo = 1 
    AND sa.Ativo = 1 AND cc.Ativo = 1 
    AND (c.Ativa = 1 OR c.Id IS NULL)
    AND (p.Ativo = 1 OR p.Id IS NULL);
```

### **View - Produtos com Receita**
```sql
CREATE VIEW [dbo].[vw_ProdutosReceita] AS
SELECT 
    p.Id as ProdutoId,
    p.Nome as ProdutoNome,
    p.Codigo as ProdutoCodigo,
    c.Nome as CategoriaNome,
    i.Nome as IngredienteNome,
    pi.Quantidade,
    pi.UnidadeMedida,
    i.UnidadeMedida as IngredienteUnidade
FROM Produtos p
    INNER JOIN Categorias c ON p.CategoriaId = c.Id
    LEFT JOIN ProdutoIngrediente pi ON p.Id = pi.ProdutoId
    LEFT JOIN Ingredientes i ON pi.IngredienteId = i.Id
WHERE p.Ativo = 1 AND c.Ativa = 1 
    AND (i.Ativo = 1 OR i.Id IS NULL);
```

### **Stored Procedure - Busca Avançada Produtos**
```sql
CREATE PROCEDURE [dbo].[sp_BuscaProdutos]
    @Nome NVARCHAR(100) = NULL,
    @Codigo NVARCHAR(20) = NULL,
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @FilialId UNIQUEIDENTIFIER = NULL,
    @Ativo BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.Id,
        p.Codigo,
        p.Nome,
        p.Descricao,
        p.UnidadeMedida,
        c.Nome as CategoriaNome,
        cc.Nome as CentroCustoNome,
        f.NomeFantasia as FilialNome,
        COUNT(*) OVER() as TotalCount
    FROM Produtos p
        INNER JOIN Categorias c ON p.CategoriaId = c.Id
        INNER JOIN CentrosCusto cc ON c.CentroCustoId = cc.Id
        INNER JOIN SubAgrupamentos sa ON cc.SubAgrupamentoId = sa.Id
        INNER JOIN Agrupamentos a ON sa.AgrupamentoId = a.Id
        INNER JOIN Filiais f ON a.FilialId = f.Id
    WHERE (@Nome IS NULL OR p.Nome LIKE '%' + @Nome + '%')
        AND (@Codigo IS NULL OR p.Codigo = @Codigo)
        AND (@CategoriaId IS NULL OR p.CategoriaId = @CategoriaId)
        AND (@FilialId IS NULL OR f.Id = @FilialId)
        AND p.Ativo = @Ativo
        AND c.Ativa = 1 AND cc.Ativo = 1 AND sa.Ativo = 1 
        AND a.Ativo = 1 AND f.Ativa = 1
    ORDER BY p.Nome
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
```

---

## 🔍 **Indexes de Performance**

### **Indexes Principais**
```sql
-- Produtos - Busca por nome (mais comum)
CREATE NONCLUSTERED INDEX [IX_Produtos_Nome_Include] 
ON [Produtos] ([Nome]) 
INCLUDE ([Codigo], [CategoriaId], [Ativo]);

-- Log Operações - Consultas por período
CREATE NONCLUSTERED INDEX [IX_LogOperacao_DataOperacao_Usuario] 
ON [LogOperacao] ([DataOperacao] DESC, [Usuario]) 
INCLUDE ([Acao], [Entidade]);

-- Auditoria por entidade
CREATE NONCLUSTERED INDEX [IX_LogOperacao_Entidade_Data] 
ON [LogOperacao] ([Entidade], [DataOperacao] DESC);

-- Performance da hierarquia
CREATE NONCLUSTERED INDEX [IX_Categorias_CentroCusto_Nivel] 
ON [Categorias] ([CentroCustoId], [Nivel]) 
INCLUDE ([Nome], [Ativa]);
```

---

## 📈 **Estatísticas e Monitoramento**

### **Query para Análise de Tamanhos**
```sql
SELECT 
    t.name AS TableName,
    s.name AS SchemaName,
    p.rows AS RowCounts,
    SUM(a.total_pages) * 8 AS TotalSpaceKB,
    SUM(a.used_pages) * 8 AS UsedSpaceKB,
    (SUM(a.total_pages) - SUM(a.used_pages)) * 8 AS UnusedSpaceKB
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.name NOT LIKE 'dt%' 
    AND t.is_ms_shipped = 0
    AND i.object_id > 255
GROUP BY t.name, s.name, p.rows
ORDER BY p.rows DESC;
```

### **Monitoramento de Performance**
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
WHERE qt.text LIKE '%Produtos%' OR qt.text LIKE '%Empresas%'
ORDER BY avg_elapsed_time DESC;
```

---

**Schema robusto, escalável e preparado para crescimento enterprise!**

*Última atualização: Setembro 2024*
# 📡 API Reference - Sistema ERP Restaurantes

Documentação completa da API REST do Sistema ERP Restaurantes desenvolvida em ASP.NET Core 9.0.

## 🔗 **Base URL e Informações Gerais**

### **URLs de Desenvolvimento**
```
Base URL: http://localhost:5268/api
Swagger UI: http://localhost:5268/swagger
Health Check: http://localhost:5268/api/health
```

### **Informações da API**
- **Versão**: 1.0.0
- **Protocolo**: HTTPS/HTTP
- **Formato**: JSON
- **Charset**: UTF-8
- **Content-Type**: `application/json`

## 🔐 **Autenticação**

### **JWT Bearer Token**
Todas as rotas protegidas requerem autenticação via JWT Bearer Token no header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### **Obter Token de Acesso**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@restaurantedemo.com",
  "senha": "Admin123!"
}
```

**Response Success (200)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiracao": "2024-09-04T18:00:00Z",
  "usuario": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nome": "Administrator",
    "email": "admin@restaurantedemo.com", 
    "perfil": "ADMIN",
    "empresaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "empresaNome": "Restaurante Demo Ltda",
    "modulosLiberados": [
      "EMPRESAS", "FILIAIS", "CENTRO_CUSTO", 
      "CATEGORIAS", "PRODUTOS"
    ],
    "filiaisAcesso": ["3fa85f64-5717-4562-b3fc-2c963f66afa6"]
  }
}
```

## 📋 **Endpoints por Módulo**

---

## 🔑 **Autenticação - /api/auth**

### **POST /auth/login**
Autentica usuário e retorna JWT token.

**Request Body**:
```json
{
  "email": "string (required, email format)",
  "senha": "string (required, min: 6 chars)"
}
```

**Responses**:
- **200 OK**: Login successful
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Invalid credentials

---

### **POST /auth/logout**
Invalida token JWT atual (logout).

**Headers**: `Authorization: Bearer {token}`

**Responses**:
- **200 OK**: Logout successful
- **401 Unauthorized**: Invalid/expired token

---

### **GET /auth/usuario**
Retorna dados do usuário logado.

**Headers**: `Authorization: Bearer {token}`

**Response (200)**:
```json
{
  "id": "guid",
  "nome": "string",
  "email": "string",
  "perfil": "string",
  "empresaId": "guid", 
  "empresaNome": "string",
  "modulosLiberados": ["string"],
  "filiaisAcesso": ["guid"]
}
```

---

### **GET /auth/modulos**
Lista módulos liberados para o usuário.

**Response (200)**:
```json
{
  "modulos": [
    "EMPRESAS",
    "FILIAIS", 
    "CENTRO_CUSTO",
    "CATEGORIAS",
    "PRODUTOS"
  ]
}
```

---

## 🏢 **Empresas - /api/empresas**

> **Authorization Required**: Módulo `EMPRESAS`

### **GET /empresas**
Lista todas as empresas ativas.

**Query Parameters**:
- `nome` (optional): Filtro por nome/razão social
- `cnpj` (optional): Filtro por CNPJ
- `ativa` (optional): true/false - filtro por status

**Response (200)**:
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "razaoSocial": "Restaurante Demo Ltda",
    "nomeFantasia": "Restaurante Demo",
    "cnpj": "12.345.678/0001-90",
    "email": "contato@restaurantedemo.com",
    "telefone": "(11) 99999-9999",
    "endereco": {
      "cep": "01234-567",
      "logradouro": "Rua Demo, 123",
      "complemento": "Sala 456",
      "bairro": "Centro",
      "cidade": "São Paulo",
      "estado": "SP"
    },
    "ativa": true,
    "dataCriacao": "2024-01-15T10:30:00Z",
    "filiais": []
  }
]
```

**Responses**:
- **200 OK**: Lista de empresas
- **204 No Content**: Nenhuma empresa encontrada
- **401 Unauthorized**: Token inválido
- **403 Forbidden**: Sem permissão para módulo

---

### **GET /empresas/{id}**
Busca empresa específica por ID.

**Path Parameters**:
- `id` (required): GUID da empresa

**Response (200)**: [Same as array item above]

**Responses**:
- **200 OK**: Empresa encontrada
- **404 Not Found**: Empresa não encontrada
- **401 Unauthorized**: Token inválido

---

### **POST /empresas**
Cria nova empresa.

**Request Body**:
```json
{
  "razaoSocial": "string (required, max: 255)",
  "nomeFantasia": "string (required, max: 255)",
  "cnpj": "string (required, format: XX.XXX.XXX/XXXX-XX)",
  "email": "string (required, email format, max: 255)",
  "telefone": "string (optional, max: 20)",
  "endereco": {
    "cep": "string (optional, format: XXXXX-XXX)",
    "logradouro": "string (optional, max: 255)",
    "numero": "string (optional, max: 10)",
    "complemento": "string (optional, max: 100)",
    "bairro": "string (optional, max: 100)",
    "cidade": "string (optional, max: 100)",
    "estado": "string (optional, max: 2)"
  }
}
```

**Response (201)**: [Created empresa object]

**Responses**:
- **201 Created**: Empresa criada com sucesso
- **400 Bad Request**: Dados inválidos
- **409 Conflict**: CNPJ ou email já existe

---

### **PUT /empresas/{id}**
Atualiza empresa existente.

**Path Parameters**:
- `id` (required): GUID da empresa

**Request Body**: [Same as POST /empresas]

**Responses**:
- **200 OK**: Empresa atualizada
- **400 Bad Request**: Dados inválidos
- **404 Not Found**: Empresa não encontrada
- **409 Conflict**: CNPJ ou email já existe

---

### **DELETE /empresas/{id}**
Desativa empresa (soft delete).

**Path Parameters**:
- `id` (required): GUID da empresa

**Responses**:
- **204 No Content**: Empresa desativada
- **404 Not Found**: Empresa não encontrada
- **400 Bad Request**: Empresa possui filiais ativas

---

## 🏪 **Filiais - /api/filiais**

> **Authorization Required**: Módulo `FILIAIS`

### **GET /filiais**
Lista filiais com filtros opcionais.

**Query Parameters**:
- `empresaId` (optional): GUID da empresa
- `nome` (optional): Filtro por nome
- `ativa` (optional): true/false

**Response (200)**:
```json
[
  {
    "id": "guid",
    "razaoSocial": "Filial Centro Ltda",
    "nomeFantasia": "Filial Centro",
    "cnpj": "12.345.678/0002-71",
    "email": "centro@restaurantedemo.com",
    "telefone": "(11) 88888-8888",
    "endereco": { /* endereco object */ },
    "ativa": true,
    "empresaId": "guid",
    "dataCriacao": "2024-01-20T14:30:00Z"
  }
]
```

### **POST /filiais**
Cria nova filial.

**Request Body**:
```json
{
  "empresaId": "guid (required)",
  "razaoSocial": "string (required, max: 255)",
  "nomeFantasia": "string (required, max: 255)", 
  "cnpj": "string (required, unique)",
  "email": "string (required, unique, email)",
  "telefone": "string (optional, max: 20)",
  "endereco": { /* endereco object optional */ }
}
```

[Similar pattern for GET/{id}, PUT/{id}, DELETE/{id}]

---

## 📊 **Centro de Custos - /api/agrupamentos**

> **Authorization Required**: Módulo `CENTRO_CUSTO`

### **GET /agrupamentos**
Lista agrupamentos por filial.

**Query Parameters**:
- `filialId` (optional): GUID da filial

**Response (200)**:
```json
[
  {
    "id": "guid",
    "codigo": "AGRUP001",
    "nome": "Cozinha",
    "descricao": "Agrupamento da cozinha",
    "ativo": true,
    "filialId": "guid",
    "dataCriacao": "2024-01-25T09:00:00Z"
  }
]
```

### **POST /agrupamentos**
Cria novo agrupamento.

**Request Body**:
```json
{
  "filialId": "guid (required)",
  "codigo": "string (required, max: 20, unique per filial)",
  "nome": "string (required, max: 100, unique per filial)",
  "descricao": "string (optional, max: 500)"
}
```

---

## **Sub Agrupamentos - /api/subagrupamentos**

### **GET /subagrupamentos**
Lista sub-agrupamentos.

**Query Parameters**:
- `agrupamentoId` (optional): GUID do agrupamento

**Response (200)**:
```json
[
  {
    "id": "guid",
    "codigo": "SUB001", 
    "nome": "Preparação",
    "descricao": "Sub-agrupamento preparação",
    "ativo": true,
    "agrupamentoId": "guid",
    "dataCriacao": "2024-01-25T10:00:00Z"
  }
]
```

---

## **Centros de Custo - /api/centroscusto**

### **GET /centroscusto**
Lista centros de custo.

**Query Parameters**:
- `subAgrupamentoId` (optional): GUID do sub-agrupamento

**Response (200)**:
```json
[
  {
    "id": "guid",
    "codigo": "CC001",
    "nome": "Fogão Industrial",
    "descricao": "Centro de custo fogão",
    "ativo": true, 
    "subAgrupamentoId": "guid",
    "dataCriacao": "2024-01-25T11:00:00Z"
  }
]
```

---

## 📂 **Categorias - /api/categorias**

> **Authorization Required**: Módulo `CATEGORIAS`

### **GET /categorias**
Lista categorias hierárquicas.

**Query Parameters**:
- `centroCustoId` (optional): GUID do centro de custo
- `categoriaPaiId` (optional): GUID da categoria pai (para subcategorias)
- `nivel` (optional): 1, 2, ou 3 (filtro por nível hierárquico)

**Response (200)**:
```json
[
  {
    "id": "guid",
    "codigo": "CAT001",
    "nome": "Bebidas",
    "descricao": "Categoria bebidas",
    "nivel": 1,
    "ativa": true,
    "centroCustoId": "guid", 
    "categoriaPaiId": null,
    "dataCriacao": "2024-01-30T08:00:00Z",
    "subcategorias": [
      {
        "id": "guid",
        "codigo": "CAT001.1",
        "nome": "Refrigerantes",
        "nivel": 2,
        "categoriaPaiId": "guid_categoria_pai"
      }
    ]
  }
]
```

### **POST /categorias**
Cria nova categoria.

**Request Body**:
```json
{
  "centroCustoId": "guid (required)",
  "categoriaPaiId": "guid (optional, for subcategories)",
  "codigo": "string (required, max: 20, unique per centro custo)",
  "nome": "string (required, max: 100, unique per centro custo)",
  "descricao": "string (optional, max: 500)"
}
```

---

## 🛍️ **Produtos - /api/produtos**

> **Authorization Required**: Módulo `PRODUTOS`

### **GET /produtos**
Lista produtos com busca e filtros avançados.

**Query Parameters**:
- `nome` (optional): Busca por nome (like)
- `codigo` (optional): Busca por código
- `categoriaId` (optional): Filtro por categoria
- `ativo` (optional): true/false
- `page` (optional): Número da página (default: 1)
- `pageSize` (optional): Itens por página (default: 20, max: 100)

**Response (200)**:
```json
{
  "items": [
    {
      "id": "guid",
      "codigo": "PROD001",
      "nome": "Coca-Cola 350ml",
      "descricao": "Refrigerante Coca-Cola lata 350ml",
      "categoriaId": "guid",
      "categoriaNome": "Refrigerantes",
      "unidadeMedida": "UN",
      "ativo": true,
      "dataCriacao": "2024-02-01T10:00:00Z",
      "ingredientes": [
        {
          "id": "guid", 
          "nome": "Xarope Coca-Cola",
          "quantidade": 50.0,
          "unidadeMedida": "ML"
        }
      ]
    }
  ],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

### **GET /produtos/{id}**
Busca produto específico com ingredientes.

**Response (200)**:
```json
{
  "id": "guid",
  "codigo": "PROD001",
  "nome": "Coca-Cola 350ml",
  "descricao": "Refrigerante Coca-Cola lata 350ml",
  "categoriaId": "guid",
  "categoriaNome": "Refrigerantes", 
  "unidadeMedida": "UN",
  "ativo": true,
  "dataCriacao": "2024-02-01T10:00:00Z",
  "ingredientes": [
    {
      "id": "guid",
      "nome": "Xarope Coca-Cola", 
      "quantidade": 50.0,
      "unidadeMedida": "ML"
    }
  ]
}
```

### **POST /produtos**
Cria novo produto.

**Request Body**:
```json
{
  "categoriaId": "guid (required)",
  "codigo": "string (required, max: 20, unique global)",
  "nome": "string (required, max: 100, unique per category)",
  "descricao": "string (optional, max: 1000)",
  "unidadeMedida": "string (required, max: 10)",
  "ingredientes": [
    {
      "nome": "string (required, max: 100)",
      "quantidade": "number (required, > 0)",
      "unidadeMedida": "string (required, max: 10)"
    }
  ]
}
```

### **PUT /produtos/{id}**
Atualiza produto existente.

**Request Body**: [Same as POST, without categoriaId]

### **DELETE /produtos/{id}**
Desativa produto (soft delete).

---

## 🏥 **Health Checks - /api/health**

### **GET /health**
Health check básico da aplicação.

**Response (200)**:
```json
{
  "status": "Healthy",
  "timestamp": "2024-09-04T12:00:00Z"
}
```

### **GET /health/live**
Liveness probe - verifica se aplicação está viva.

### **GET /health/ready**  
Readiness probe - verifica se aplicação está pronta.

### **GET /health/detailed**
Health check detalhado com todos os serviços.

**Response (200)**:
```json
{
  "status": "Healthy",
  "timestamp": "2024-09-04T12:00:00Z",
  "checks": {
    "database": {
      "status": "Healthy",
      "responseTime": "15ms"
    },
    "cache": {
      "status": "Healthy", 
      "responseTime": "2ms"
    },
    "system": {
      "status": "Healthy",
      "memoryUsage": "45%",
      "cpuUsage": "12%"
    }
  }
}
```

---

## 📊 **Métricas - /api/metrics**

### **GET /metrics**
Métricas básicas da aplicação.

**Response (200)**:
```json
{
  "totalRequests": 15420,
  "averageResponseTime": "125ms",
  "errorRate": "0.2%",
  "uptime": "15d 4h 32m",
  "activeUsers": 23
}
```

### **GET /metrics/performance** 
Métricas de performance detalhadas.

**Response (200)**:
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
      {
        "endpoint": "/api/produtos",
        "averageTime": "280ms"
      }
    ]
  }
}
```

---

## ⚠️ **Error Responses**

### **Padrão de Erros**
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    "Detailed error 1",
    "Detailed error 2"  
  ],
  "timestamp": "2024-09-04T12:00:00Z",
  "path": "/api/empresas"
}
```

### **Status Codes Comuns**
- **200 OK**: Requisição bem-sucedida
- **201 Created**: Recurso criado com sucesso
- **204 No Content**: Operação bem-sucedida sem conteúdo
- **400 Bad Request**: Dados inválidos ou malformados
- **401 Unauthorized**: Token ausente ou inválido
- **403 Forbidden**: Sem permissão para acessar recurso
- **404 Not Found**: Recurso não encontrado
- **409 Conflict**: Conflito de dados (duplicate)
- **422 Unprocessable Entity**: Erros de validação
- **500 Internal Server Error**: Erro interno do servidor

### **Validation Error Response**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "RazaoSocial": ["Razão social é obrigatória"],
    "Cnpj": ["CNPJ deve ter formato válido"],
    "Email": ["Email já existe no sistema"]
  }
}
```

---

## 🚀 **Rate Limiting**

- **Limit**: 100 requests/minute por IP
- **Auth endpoints**: 10 requests/minute
- **Headers** de resposta:
  ```
  X-RateLimit-Limit: 100
  X-RateLimit-Remaining: 85
  X-RateLimit-Reset: 1609459200
  ```

---

## 📋 **Postman Collection**

Uma collection do Postman com todos os endpoints está disponível em:
`docs/api/GestaoRestaurante.postman_collection.json`

### **Ambiente Postman**
```json
{
  "name": "Gestão Restaurante - Dev",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5268/api"
    },
    {
      "key": "token", 
      "value": "{{authToken}}"
    }
  ]
}
```

---

## 🔧 **SDK e Integrações**

### **cURL Examples**

**Login**:
```bash
curl -X POST http://localhost:5268/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@restaurantedemo.com","senha":"Admin123!"}'
```

**Get Empresas**:
```bash
curl -X GET http://localhost:5268/api/empresas \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Create Empresa**:
```bash
curl -X POST http://localhost:5268/api/empresas \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "razaoSocial": "Nova Empresa Ltda",
    "nomeFantasia": "Nova Empresa", 
    "cnpj": "98.765.432/0001-10",
    "email": "contato@novaempresa.com"
  }'
```

---

**API completa, documentada e pronta para integração!**
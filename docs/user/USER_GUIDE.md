# 📖 Manual do Usuário - Sistema ERP Restaurantes

Guia completo para utilização do Sistema ERP de Gestão de Restaurantes.

## 🎯 **Introdução**

O Sistema ERP Restaurantes é uma solução completa para gestão de restaurantes, oferecendo controle total sobre empresas, filiais, produtos, categorias e estrutura organizacional.

### **O que você pode fazer**
- ✅ Gerenciar empresas e filiais
- ✅ Organizar produtos por categorias hierárquicas
- ✅ Controlar centros de custo
- ✅ Gerenciar usuários e permissões
- ✅ Visualizar métricas e relatórios básicos

## 🚀 **Primeiros Passos**

### **1. Acessando o Sistema**
1. Abra seu navegador e acesse: `http://localhost:5173`
2. Você será direcionado para a tela de login

### **2. Fazendo Login**
1. **Email**: Digite seu email cadastrado
2. **Senha**: Digite sua senha
3. Clique em **"Entrar"**

**Credenciais de Demonstração**:
- Email: `admin@restaurantedemo.com`
- Senha: `Admin123!`

### **3. Navegando pela Interface**
Após o login, você verá:

- **Sidebar (Menu Lateral)**: Navegação principal do sistema
- **Header (Cabeçalho)**: Informações do usuário e logout
- **Dashboard**: Métricas e atalhos rápidos
- **Área de Conteúdo**: Páginas específicas de cada módulo

## 🏠 **Dashboard Principal**

### **Visão Geral**
O Dashboard é sua tela inicial, mostrando:

**Cards de Informação**:
- 🏢 **Empresa Ativa**: Nome da empresa atual
- ✅ **Módulos Liberados**: Quantidade de módulos disponíveis
- 🏪 **Filiais Acesso**: Filiais que você pode acessar
- 👤 **Perfil de Acesso**: Seu nível de permissão

**Atalhos Rápidos**:
- Links diretos para funcionalidades principais
- Acesso rápido a cadastros mais utilizados

**Módulos Liberados**:
- Lista dos módulos que seu perfil pode acessar
- Status ativo/inativo de cada módulo

## 📋 **Navegação e Menu**

### **Menu Principal (Sidebar)**
Clique no **ícone do hambúrguer** (☰) para expandir/recolher o menu.

**Estrutura do Menu**:
```
🏠 Dashboard
📁 Cadastros
  ├── 🏢 Empresas
  ├── 🏪 Filiais  
  ├── 📊 Agrupamentos
  ├── 📋 Sub Agrupamentos
  └── 💰 Centros de Custo
```

### **Breadcrumbs**
Na parte superior de cada página, você encontrará o caminho atual:
`Home > Cadastros > Empresas`

## 🏢 **Módulo Empresas**

### **Listagem de Empresas**
**Navegação**: Menu > Cadastros > Empresas

**Funcionalidades**:
- ✅ Visualizar todas as empresas cadastradas
- 🔍 Buscar empresas por nome ou CNPJ
- ➕ Criar nova empresa
- ✏️ Editar empresa existente
- 🗑️ Desativar empresa

### **Como Criar uma Empresa**
1. Acesse **Cadastros > Empresas**
2. Clique no botão **"Nova Empresa"**
3. Preencha os campos obrigatórios:
   - **Razão Social**: Nome oficial da empresa
   - **Nome Fantasia**: Nome comercial
   - **CNPJ**: Documento da empresa (será formatado automaticamente)
   - **Email**: Email de contato
4. Campos opcionais:
   - **Telefone**: Telefone de contato
   - **Endereço**: Endereço completo
5. Clique em **"Salvar"**

### **Editando uma Empresa**
1. Na lista de empresas, clique no **ícone de edição** (✏️)
2. Modifique os dados necessários
3. Clique em **"Salvar"**

### **Desativando uma Empresa**
1. Na lista de empresas, clique no **ícone de lixeira** (🗑️)
2. Confirme a ação no modal que aparecer
3. A empresa será **desativada** (não excluída)

> ⚠️ **Importante**: Uma empresa só pode ser desativada se não tiver filiais ativas.

## 🏪 **Módulo Filiais**

### **Gestão de Filiais**
**Navegação**: Menu > Cadastros > Filiais

As filiais são unidades vinculadas a uma empresa principal.

### **Criando uma Filial**
1. Acesse **Cadastros > Filiais**
2. Clique em **"Nova Filial"**
3. Preencha:
   - **Empresa**: Selecione a empresa proprietária
   - **Razão Social**: Nome oficial da filial
   - **Nome Fantasia**: Nome comercial da filial
   - **CNPJ**: CNPJ específico da filial
   - **Email**: Email da filial
   - **Endereço**: Localização da filial
4. Salve as informações

### **Vinculação Empresa-Filial**
- Cada filial pertence a **uma única empresa**
- Uma empresa pode ter **múltiplas filiais**
- Filiais herdam configurações básicas da empresa matriz

## 📊 **Estrutura de Centro de Custos**

O sistema utiliza uma estrutura hierárquica de 3 níveis:

```
🏪 Filial
└── 📊 Agrupamento (Ex: "Cozinha")
    └── 📋 Sub Agrupamento (Ex: "Preparação")
        └── 💰 Centro de Custo (Ex: "Fogão Industrial")
```

### **Agrupamentos**
**Navegação**: Menu > Cadastros > Agrupamentos

**Função**: Grandes divisões da filial (Cozinha, Salão, Administrativo)

### **Sub Agrupamentos**
**Navegação**: Menu > Cadastros > Sub Agrupamentos

**Função**: Subdivisões dos agrupamentos (Preparação, Limpeza, Estoque)

### **Centros de Custo**
**Navegação**: Menu > Cadastros > Centros de Custo

**Função**: Pontos específicos de custo (Fogão, Geladeira, Mesa 1)

### **Criando a Hierarquia**
1. **Primeiro**: Crie os Agrupamentos
2. **Segundo**: Crie os Sub Agrupamentos vinculados aos Agrupamentos
3. **Terceiro**: Crie os Centros de Custo vinculados aos Sub Agrupamentos

## 📂 **Módulo Categorias**

### **Sistema de Categorias**
**Navegação**: Menu > Cadastros > Categorias

As categorias organizam produtos em até **3 níveis hierárquicos**:

```
📂 Bebidas (Nível 1)
├── 🥤 Refrigerantes (Nível 2)
│   ├── Cola (Nível 3)
│   └── Limão (Nível 3)
└── 🍺 Cervejas (Nível 2)
    ├── Pilsen (Nível 3)
    └── IPA (Nível 3)
```

### **Criando Categorias**
1. **Categoria Principal (Nível 1)**:
   - Centro de Custo: Selecione onde a categoria se aplica
   - Código: Código único (ex: "BEB001")
   - Nome: Nome da categoria (ex: "Bebidas")

2. **Subcategoria (Nível 2)**:
   - Categoria Pai: Selecione a categoria de nível 1
   - Código: Código sequencial (ex: "BEB001.1")
   - Nome: Nome da subcategoria (ex: "Refrigerantes")

3. **Sub-subcategoria (Nível 3)**:
   - Categoria Pai: Selecione a categoria de nível 2
   - Código: Código sequencial (ex: "BEB001.1.1")
   - Nome: Nome específico (ex: "Cola")

## 🛍️ **Módulo Produtos**

### **Gestão de Produtos**
**Navegação**: Menu > Cadastros > Produtos

### **Cadastrando um Produto**
1. Clique em **"Novo Produto"**
2. Preencha as informações:
   - **Código**: Código único do produto (ex: "PROD001")
   - **Nome**: Nome do produto (ex: "Coca-Cola 350ml")
   - **Categoria**: Selecione a categoria (preferencialmente nível 3)
   - **Unidade de Medida**: UN, KG, LT, ML, etc.
   - **Descrição**: Descrição detalhada (opcional)

### **Ingredientes do Produto**
Para produtos compostos, você pode cadastrar ingredientes:

1. Na seção **"Ingredientes"**, clique em **"Adicionar Ingrediente"**
2. Preencha:
   - **Nome**: Nome do ingrediente
   - **Quantidade**: Quantidade utilizada
   - **Unidade**: Unidade de medida do ingrediente
3. Adicione quantos ingredientes necessários
4. Salve o produto

**Exemplo - Hambúrguer**:
- Pão: 1 UN
- Carne: 120 GR
- Queijo: 1 FATIA
- Alface: 2 FOLHAS

## 👤 **Gerenciamento de Usuários**

### **Perfis de Acesso**
O sistema possui diferentes perfis:

- **ADMIN**: Acesso total ao sistema
- **GERENTE**: Acesso a múltulos módulos
- **OPERADOR**: Acesso limitado a módulos específicos
- **USUARIO**: Acesso básico

### **Módulos por Perfil**
Cada usuário tem acesso apenas aos módulos liberados:

- **EMPRESAS**: Gestão de empresas
- **FILIAIS**: Gestão de filiais
- **CENTRO_CUSTO**: Estrutura de custos
- **CATEGORIAS**: Gestão de categorias
- **PRODUTOS**: Gestão de produtos

### **Visualizando Suas Permissões**
1. No **Dashboard**, veja o card **"Módulos Liberados"**
2. No **Header**, clique no seu nome para ver informações do perfil

## 🔍 **Funcionalidades Gerais**

### **Busca e Filtros**
Nas listagens, você pode:
- 🔍 **Buscar**: Digite termos na caixa de busca
- 🎛️ **Filtrar**: Use filtros por status, categoria, etc.
- 📄 **Paginar**: Navegue entre páginas de resultados

### **Ordenação**
Clique nos cabeçalhos das tabelas para ordenar:
- 📈 **Crescente**: Primeira clicada
- 📉 **Decrescente**: Segunda clicada
- ↕️ **Neutro**: Terceira clicada

### **Ações em Massa**
Em algumas telas, você pode:
- ☑️ Selecionar múltiplos itens
- 🔄 Aplicar ações em lote
- 📊 Exportar dados selecionados

### **Notificações**
O sistema exibe notificações:
- ✅ **Sucesso**: Ações realizadas com êxito
- ❌ **Erro**: Problemas encontrados
- ⚠️ **Aviso**: Informações importantes
- ℹ️ **Informação**: Mensagens gerais

## ⚠️ **Validações e Regras**

### **Campos Obrigatórios**
Campos marcados com **asterisco (*)** são obrigatórios.

### **Formatos Específicos**
- **CNPJ**: Formato XX.XXX.XXX/XXXX-XX
- **Email**: Formato email válido
- **CEP**: Formato XXXXX-XXX
- **Telefone**: Formato (XX) XXXXX-XXXX

### **Regras de Negócio**
- **CNPJ único**: Cada CNPJ só pode ser cadastrado uma vez
- **Email único**: Emails não podem ser duplicados
- **Hierarquia**: Respeitar estrutura de níveis
- **Dependências**: Não é possível excluir itens com dependências

## 🆘 **Solução de Problemas**

### **Problemas Comuns**

**❌ "Erro ao fazer login"**
- Verifique email e senha
- Certifique-se de que o usuário está ativo
- Contate o administrador se persistir

**❌ "Acesso negado ao módulo"**
- Seu perfil não tem acesso a este módulo
- Contate o administrador para liberação

**❌ "CNPJ já cadastrado"**
- O CNPJ informado já existe no sistema
- Verifique se não é duplicação
- Use CNPJ diferente para filiais

**❌ "Não é possível excluir"**
- O item possui dependências (ex: empresa com filiais)
- Remova as dependências primeiro
- Ou desative o item em vez de excluir

### **Performance Lenta**
- Feche outras abas do navegador
- Limpe o cache do navegador
- Verifique sua conexão com a internet
- Use filtros para reduzir dados carregados

### **Dados não Aparecem**
- Verifique os filtros aplicados
- Confirme se tem permissão para visualizar
- Atualize a página (F5)

## 🔒 **Segurança**

### **Boas Práticas**
- ✅ Sempre faça logout ao sair
- ✅ Use senhas fortes e únicas
- ✅ Não compartilhe suas credenciais
- ✅ Mantenha seu navegador atualizado

### **Sessão e Timeout**
- Sua sessão expira em **8 horas**
- Será deslogado automaticamente por segurança
- Salve trabalhos longos periodicamente

### **Permissões**
- Você só vê dados que tem permissão
- Algumas ações podem ser restritas por perfil
- Módulos inacessíveis não aparecem no menu

## 📞 **Suporte**

### **Em Caso de Dúvidas**
1. Consulte este manual primeiro
2. Verifique o [FAQ](./FAQ.md)
3. Contate o suporte técnico
4. Fale com seu administrador do sistema

### **Reportando Problemas**
Ao reportar problemas, informe:
- **O que estava fazendo**: Ação que tentou realizar
- **Mensagem de erro**: Texto exato do erro
- **Navegador**: Chrome, Firefox, Edge, etc.
- **Horário**: Quando ocorreu o problema

---

**Sistema intuitivo e fácil de usar. Em caso de dúvidas, não hesite em pedir ajuda!**
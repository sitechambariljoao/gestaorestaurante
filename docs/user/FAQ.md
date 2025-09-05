# ❓ FAQ - Perguntas Frequentes

Respostas para as dúvidas mais comuns sobre o Sistema ERP Restaurantes.

## 🔐 **Login e Acesso**

### **❓ Como faço login no sistema?**
**R:** Acesse http://localhost:5173 e use suas credenciais. Para demonstração, use:
- Email: `admin@restaurantedemo.com`
- Senha: `Admin123!`

### **❓ Esqueci minha senha, como recupero?**
**R:** Atualmente o sistema não possui recuperação automática de senha. Entre em contato com o administrador do sistema para redefinir sua senha.

### **❓ Por quanto tempo fico logado?**
**R:** Sua sessão expira automaticamente em **8 horas**. Após esse período, será necessário fazer login novamente.

### **❓ Posso acessar de múltiplos dispositivos?**
**R:** Sim, você pode fazer login em múltiplos navegadores/dispositivos simultaneamente com as mesmas credenciais.

---

## 👤 **Usuários e Permissões**

### **❓ Não consigo acessar um módulo, por quê?**
**R:** Seu perfil de usuário não tem permissão para esse módulo. Verifique no Dashboard quais módulos estão liberados para você. Contate o administrador para solicitar acesso.

### **❓ Quais são os perfis disponíveis?**
**R:** O sistema possui 4 perfis:
- **ADMIN**: Acesso total
- **GERENTE**: Acesso a múltiplos módulos
- **OPERADOR**: Acesso limitado (produtos, categorias)
- **USUARIO**: Acesso básico (visualização)

### **❓ Como altero minha senha?**
**R:** Clique no seu nome no canto superior direito → "Configurações" → "Alterar Senha". (Funcionalidade em desenvolvimento)

### **❓ Posso acessar dados de outras empresas?**
**R:** Não. O sistema é multi-tenant, você só acessa dados da sua empresa e filiais autorizadas.

---

## 🏢 **Empresas e Filiais**

### **❓ Posso cadastrar uma empresa sem CNPJ?**
**R:** Não, o CNPJ é obrigatório e deve ser válido. O sistema valida automaticamente o CNPJ digitado.

### **❓ Por que não consigo excluir uma empresa?**
**R:** Uma empresa só pode ser excluída se não tiver:
- Filiais ativas vinculadas
- Usuários associados
- Movimentações financeiras (futuro)
Você pode **desativar** a empresa em vez de excluir.

### **❓ Uma filial pode ter CNPJ igual à matriz?**
**R:** Não, cada CNPJ deve ser único no sistema. Filiais precisam ter CNPJ próprio (geralmente termina com /0002-XX, /0003-XX, etc.)

### **❓ Posso mover uma filial para outra empresa?**
**R:** Não é possível através da interface. Entre em contato com o administrador para operações especiais como esta.

---

## 📊 **Centro de Custos**

### **❓ Qual a diferença entre Agrupamento, Sub Agrupamento e Centro de Custo?**
**R:** É uma estrutura hierárquica:
- **Agrupamento**: Grandes divisões (ex: Cozinha, Salão)
- **Sub Agrupamento**: Subdivisões (ex: Preparação, Limpeza)
- **Centro de Custo**: Pontos específicos (ex: Fogão, Mesa 1)

### **❓ Posso ter o mesmo código em filiais diferentes?**
**R:** Sim, códigos de agrupamentos podem repetir entre filiais, mas devem ser únicos dentro da mesma filial.

### **❓ O que acontece se excluir um agrupamento?**
**R:** Você só conseguirá excluir se não houver:
- Sub agrupamentos vinculados
- Centros de custo vinculados
- Categorias vinculadas
O sistema sempre verifica dependências antes de permitir exclusões.

---

## 📂 **Categorias e Produtos**

### **❓ Quantos níveis de categoria posso criar?**
**R:** Máximo de **3 níveis**:
1. Categoria principal (ex: Bebidas)
2. Subcategoria (ex: Refrigerantes)
3. Sub-subcategoria (ex: Cola)

### **❓ Posso mover um produto para outra categoria?**
**R:** Sim, edite o produto e selecione a nova categoria. O histórico da mudança fica registrado no sistema.

### **❓ Como funciona o código de produtos?**
**R:** 
- Códigos de **produtos** são únicos **globalmente** (não podem repetir em lugar nenhum)
- Códigos de **categorias** são únicos por centro de custo
- O sistema valida automaticamente na digitação

### **❓ Por que meu produto não aparece na listagem?**
**R:** Verifique:
- Se o produto está **ativo**
- Se você tem **permissão** para ver produtos dessa categoria
- Se há **filtros** aplicados na busca
- Se está na **página correta** da paginação

---

## 🔍 **Busca e Filtros**

### **❓ Como faço busca parcial?**
**R:** Digite parte do nome na caixa de busca. O sistema busca automaticamente por nomes que contenham o termo digitado (não precisa ser exato).

### **❓ Posso buscar por múltiplos campos?**
**R:** Depende da tela. Na maioria das listagens você pode filtrar por status (ativo/inativo) e buscar por nome simultaneamente.

### **❓ Como limpo os filtros aplicados?**
**R:** Clique no botão "Limpar Filtros" ou remova o conteúdo dos campos de filtro e pressione Enter.

---

## 💾 **Dados e Backup**

### **❓ Meus dados ficam salvos automaticamente?**
**R:** Sim, todas as alterações são salvas imediatamente no banco de dados após você clicar em "Salvar".

### **❓ Posso recuperar um registro excluído?**
**R:** O sistema usa "soft delete" (exclusão lógica). Dados excluídos podem ser recuperados pelo administrador, mas não através da interface normal.

### **❓ Com que frequência é feito backup?**
**R:** Isso depende da configuração do administrador. Recomendamos backup diário automático.

---

## 🛠️ **Problemas Técnicos**

### **❓ O sistema está lento, o que fazer?**
**R:** Tente:
1. Fechar outras abas do navegador
2. Limpar cache do navegador (Ctrl+F5)
3. Usar filtros para reduzir dados carregados
4. Verificar sua conexão com internet
5. Contatar suporte se persistir

### **❓ Apareceu um erro "500", o que significa?**
**R:** Erro 500 indica problema interno do servidor. Tente:
1. Atualizar a página (F5)
2. Fazer logout e login novamente
3. Se persistir, contate o suporte técnico

### **❓ Não consigo fazer upload de arquivos**
**R:** Verifique:
- Tamanho do arquivo (máximo 5MB)
- Formato permitido (PDF, JPG, PNG)
- Sua conexão com internet
- Se tem permissão para essa operação

### **❓ A página não carrega completamente**
**R:** Pode ser problema de conectividade ou navegador:
1. Teste em outro navegador
2. Desabilite extensões do navegador
3. Verifique se JavaScript está habilitado
4. Limpe cookies e cache

---

## 📱 **Compatibilidade**

### **❓ Funciona no celular?**
**R:** Sim, o sistema é responsivo e funciona em dispositivos móveis, mas a experiência completa é otimizada para desktop/tablet.

### **❓ Quais navegadores são suportados?**
**R:** Navegadores modernos:
- ✅ Chrome 90+
- ✅ Firefox 88+
- ✅ Edge 90+
- ✅ Safari 14+
- ❌ Internet Explorer (não suportado)

### **❓ Precisa instalar algum plugin?**
**R:** Não, o sistema roda completamente no navegador, sem necessidade de plugins ou instalações adicionais.

---

## 📊 **Relatórios e Exportação**

### **❓ Posso exportar dados?**
**R:** A funcionalidade de exportação está em desenvolvimento. Atualmente você pode copiar dados das tabelas manualmente.

### **❓ Como imprimo uma listagem?**
**R:** Use Ctrl+P do navegador. O sistema detecta automaticamente impressão e otimiza o layout.

### **❓ Existem relatórios prontos?**
**R:** O módulo de relatórios está em desenvolvimento. Atualmente você pode visualizar métricas básicas no Dashboard.

---

## 🔄 **Integrações**

### **❓ O sistema integra com outros softwares?**
**R:** O sistema possui API REST completa que permite integrações. Entre em contato com suporte técnico para detalhes específicos.

### **❓ Posso importar dados de outro sistema?**
**R:** Sim, através de scripts de importação ou pela API. Contate o administrador para planejamento de migração.

---

## 🆘 **Suporte**

### **❓ Como entro em contato com suporte?**
**R:** Hierarquia de suporte:
1. **FAQ** (este documento)
2. **Manual do Usuário** (documentação completa)
3. **Administrador local** (usuário ADMIN da sua empresa)
4. **Suporte técnico** (para problemas de sistema)

### **❓ Que informações devo fornecer ao reportar um problema?**
**R:** Inclua sempre:
- Descrição detalhada do problema
- Passos para reproduzir o erro
- Mensagem de erro exata
- Navegador e versão
- Horário que ocorreu
- Seu usuário/perfil

### **❓ Existe treinamento disponível?**
**R:** Sim:
- **Manual do Usuário**: Documentação completa
- **Vídeos tutoriais**: Em desenvolvimento
- **Treinamento presencial**: Contate administrador
- **Suporte remoto**: Disponível conforme necessidade

---

## 🚀 **Futuras Funcionalidades**

### **❓ Quais funcionalidades estão sendo desenvolvidas?**
**R:** Roadmap inclui:
- 📋 **Módulo Pedidos**: Gestão completa de pedidos
- 📦 **Módulo Estoque**: Controle de estoque
- 💰 **Módulo Financeiro**: Fluxo de caixa
- 👥 **Módulo RH**: Gestão de funcionários
- 📊 **Relatórios Avançados**: Dashboards e analytics
- 📱 **App Mobile**: Aplicativo nativo

### **❓ Como sugerir melhorias?**
**R:** Entre em contato com:
1. Administrador do sistema (funcionalidades)
2. Suporte técnico (problemas/bugs)
3. Gerente do projeto (sugestões estratégicas)

---

## ✅ **Checklist de Verificação Rápida**

**Antes de entrar em contato com suporte, verifique:**

- [ ] Tentei atualizar a página (F5)
- [ ] Tentei fazer logout e login novamente  
- [ ] Testei em outro navegador
- [ ] Verifiquei se tenho permissão para a operação
- [ ] Li a mensagem de erro completamente
- [ ] Consultei este FAQ
- [ ] Consultei o Manual do Usuário

---

**💡 Dica: Mantenha este FAQ nos favoritos para consulta rápida!**

*Documentação atualizada regularmente. Última atualização: Setembro 2024*
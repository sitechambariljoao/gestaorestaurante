using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoRestaurante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
                table: "AspNetRoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaim_Usuario_UserId",
                table: "AspNetUserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogin_Usuario_UserId",
                table: "AspNetUserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRole_AspNetRole_RoleId",
                table: "AspNetUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRole_Usuario_UserId",
                table: "AspNetUserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserToken_Usuario_UserId",
                table: "AspNetUserToken");

            migrationBuilder.DropForeignKey(
                name: "FK_AssinaturasEmpresa_Empresa_EmpresaId",
                table: "AssinaturasEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_AssinaturasEmpresa_PlanosAssinatura_PlanoId",
                table: "AssinaturasEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensPedido_Pedido_PedidoId",
                table: "ItensPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensPedido_Produto_ProdutoId",
                table: "ItensPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsOperacao_Empresa_EmpresaId",
                table: "LogsOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsOperacao_Filial_FilialId",
                table: "LogsOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsOperacao_Usuario_UsuarioId",
                table: "LogsOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_ModulosPlano_PlanosAssinatura_PlanoId",
                table: "ModulosPlano");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesEstoque_Filial_FilialId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesEstoque_Ingrediente_IngredienteId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesEstoque_Produto_ProdutoId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesEstoque_Usuario_UsuarioId",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesFinanceira_Filial_FilialId",
                table: "MovimentacoesFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesFinanceira_Pedido_PedidoId",
                table: "MovimentacoesFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacoesFinanceira_Usuario_UsuarioId",
                table: "MovimentacoesFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosJornada_Funcionario_FuncionarioId",
                table: "RegistrosJornada");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrosJornada",
                table: "RegistrosJornada");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanosAssinatura",
                table: "PlanosAssinatura");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovimentacoesFinanceira",
                table: "MovimentacoesFinanceira");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovimentacoesEstoque",
                table: "MovimentacoesEstoque");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModulosPlano",
                table: "ModulosPlano");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogsOperacao",
                table: "LogsOperacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItensPedido",
                table: "ItensPedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssinaturasEmpresa",
                table: "AssinaturasEmpresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserToken",
                table: "AspNetUserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRole",
                table: "AspNetUserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogin",
                table: "AspNetUserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaim",
                table: "AspNetUserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaim",
                table: "AspNetRoleClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRole",
                table: "AspNetRole");

            migrationBuilder.EnsureSchema(
                name: "CentroCusto");

            migrationBuilder.EnsureSchema(
                name: "Core");

            migrationBuilder.EnsureSchema(
                name: "Categorias");

            migrationBuilder.EnsureSchema(
                name: "Empresas");

            migrationBuilder.EnsureSchema(
                name: "Filiais");

            migrationBuilder.EnsureSchema(
                name: "Funcionarios");

            migrationBuilder.EnsureSchema(
                name: "Produtos");

            migrationBuilder.EnsureSchema(
                name: "Pedidos");

            migrationBuilder.EnsureSchema(
                name: "Cardapio");

            migrationBuilder.EnsureSchema(
                name: "Estoque");

            migrationBuilder.EnsureSchema(
                name: "Financeiro");

            migrationBuilder.RenameTable(
                name: "UsuarioFilial",
                newName: "UsuarioFilial",
                newSchema: "Filiais");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuario",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "SubAgrupamento",
                newName: "SubAgrupamento",
                newSchema: "CentroCusto");

            migrationBuilder.RenameTable(
                name: "ProdutoIngrediente",
                newName: "ProdutoIngrediente",
                newSchema: "Produtos");

            migrationBuilder.RenameTable(
                name: "Produto",
                newName: "Produto",
                newSchema: "Produtos");

            migrationBuilder.RenameTable(
                name: "Pedido",
                newName: "Pedido",
                newSchema: "Pedidos");

            migrationBuilder.RenameTable(
                name: "Mesa",
                newName: "Mesa",
                newSchema: "Cardapio");

            migrationBuilder.RenameTable(
                name: "Ingrediente",
                newName: "Ingrediente",
                newSchema: "Produtos");

            migrationBuilder.RenameTable(
                name: "Funcionario",
                newName: "Funcionario",
                newSchema: "Funcionarios");

            migrationBuilder.RenameTable(
                name: "FilialAgrupamento",
                newName: "FilialAgrupamento",
                newSchema: "CentroCusto");

            migrationBuilder.RenameTable(
                name: "Filial",
                newName: "Filial",
                newSchema: "Filiais");

            migrationBuilder.RenameTable(
                name: "Empresa",
                newName: "Empresa",
                newSchema: "Empresas");

            migrationBuilder.RenameTable(
                name: "CentroCusto",
                newName: "CentroCusto",
                newSchema: "CentroCusto");

            migrationBuilder.RenameTable(
                name: "Categoria",
                newName: "Categoria",
                newSchema: "Categorias");

            migrationBuilder.RenameTable(
                name: "Agrupamento",
                newName: "Agrupamento",
                newSchema: "CentroCusto");

            migrationBuilder.RenameTable(
                name: "RegistrosJornada",
                newName: "RegistroJornada",
                newSchema: "Funcionarios");

            migrationBuilder.RenameTable(
                name: "PlanosAssinatura",
                newName: "PlanoAssinatura",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "MovimentacoesFinanceira",
                newName: "MovimentacaoFinanceira",
                newSchema: "Financeiro");

            migrationBuilder.RenameTable(
                name: "MovimentacoesEstoque",
                newName: "MovimentacaoEstoque",
                newSchema: "Estoque");

            migrationBuilder.RenameTable(
                name: "ModulosPlano",
                newName: "ModuloPlano",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "LogsOperacao",
                newName: "LogOperacao",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "ItensPedido",
                newName: "ItemPedido",
                newSchema: "Pedidos");

            migrationBuilder.RenameTable(
                name: "AssinaturasEmpresa",
                newName: "AssinaturaEmpresa",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetUserToken",
                newName: "UserToken",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetUserRole",
                newName: "UserRole",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogin",
                newName: "UserLogin",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaim",
                newName: "UserClaim",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaim",
                newName: "RoleClaim",
                newSchema: "Core");

            migrationBuilder.RenameTable(
                name: "AspNetRole",
                newName: "Role",
                newSchema: "Core");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosJornada_FuncionarioId",
                schema: "Funcionarios",
                table: "RegistroJornada",
                newName: "IX_RegistroJornada_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesFinanceira_UsuarioId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                newName: "IX_MovimentacaoFinanceira_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesFinanceira_PedidoId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                newName: "IX_MovimentacaoFinanceira_PedidoId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesFinanceira_FilialId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                newName: "IX_MovimentacaoFinanceira_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesEstoque_UsuarioId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                newName: "IX_MovimentacaoEstoque_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                newName: "IX_MovimentacaoEstoque_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesEstoque_IngredienteId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                newName: "IX_MovimentacaoEstoque_IngredienteId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacoesEstoque_FilialId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                newName: "IX_MovimentacaoEstoque_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_ModulosPlano_PlanoId",
                schema: "Core",
                table: "ModuloPlano",
                newName: "IX_ModuloPlano_PlanoId");

            migrationBuilder.RenameIndex(
                name: "IX_LogsOperacao_UsuarioId",
                schema: "Core",
                table: "LogOperacao",
                newName: "IX_LogOperacao_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_LogsOperacao_FilialId",
                schema: "Core",
                table: "LogOperacao",
                newName: "IX_LogOperacao_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_LogsOperacao_EmpresaId",
                schema: "Core",
                table: "LogOperacao",
                newName: "IX_LogOperacao_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPedido_ProdutoId",
                schema: "Pedidos",
                table: "ItemPedido",
                newName: "IX_ItemPedido_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItensPedido_PedidoId",
                schema: "Pedidos",
                table: "ItemPedido",
                newName: "IX_ItemPedido_PedidoId");

            migrationBuilder.RenameIndex(
                name: "IX_AssinaturasEmpresa_PlanoId",
                schema: "Core",
                table: "AssinaturaEmpresa",
                newName: "IX_AssinaturaEmpresa_PlanoId");

            migrationBuilder.RenameIndex(
                name: "IX_AssinaturasEmpresa_EmpresaId",
                schema: "Core",
                table: "AssinaturaEmpresa",
                newName: "IX_AssinaturaEmpresa_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRole_RoleId",
                schema: "Core",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogin_UserId",
                schema: "Core",
                table: "UserLogin",
                newName: "IX_UserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaim_UserId",
                schema: "Core",
                table: "UserClaim",
                newName: "IX_UserClaim_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaim_RoleId",
                schema: "Core",
                table: "RoleClaim",
                newName: "IX_RoleClaim_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistroJornada",
                schema: "Funcionarios",
                table: "RegistroJornada",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanoAssinatura",
                schema: "Core",
                table: "PlanoAssinatura",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovimentacaoFinanceira",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovimentacaoEstoque",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuloPlano",
                schema: "Core",
                table: "ModuloPlano",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogOperacao",
                schema: "Core",
                table: "LogOperacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemPedido",
                schema: "Pedidos",
                table: "ItemPedido",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssinaturaEmpresa",
                schema: "Core",
                table: "AssinaturaEmpresa",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserToken",
                schema: "Core",
                table: "UserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                schema: "Core",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogin",
                schema: "Core",
                table: "UserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaim",
                schema: "Core",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaim",
                schema: "Core",
                table: "RoleClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                schema: "Core",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssinaturaEmpresa_Empresa_EmpresaId",
                schema: "Core",
                table: "AssinaturaEmpresa",
                column: "EmpresaId",
                principalSchema: "Empresas",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssinaturaEmpresa_PlanoAssinatura_PlanoId",
                schema: "Core",
                table: "AssinaturaEmpresa",
                column: "PlanoId",
                principalSchema: "Core",
                principalTable: "PlanoAssinatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedido_Pedido_PedidoId",
                schema: "Pedidos",
                table: "ItemPedido",
                column: "PedidoId",
                principalSchema: "Pedidos",
                principalTable: "Pedido",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPedido_Produto_ProdutoId",
                schema: "Pedidos",
                table: "ItemPedido",
                column: "ProdutoId",
                principalSchema: "Produtos",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogOperacao_Empresa_EmpresaId",
                schema: "Core",
                table: "LogOperacao",
                column: "EmpresaId",
                principalSchema: "Empresas",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogOperacao_Filial_FilialId",
                schema: "Core",
                table: "LogOperacao",
                column: "FilialId",
                principalSchema: "Filiais",
                principalTable: "Filial",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogOperacao_Usuario_UsuarioId",
                schema: "Core",
                table: "LogOperacao",
                column: "UsuarioId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuloPlano_PlanoAssinatura_PlanoId",
                schema: "Core",
                table: "ModuloPlano",
                column: "PlanoId",
                principalSchema: "Core",
                principalTable: "PlanoAssinatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoEstoque_Filial_FilialId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                column: "FilialId",
                principalSchema: "Filiais",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoEstoque_Ingrediente_IngredienteId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                column: "IngredienteId",
                principalSchema: "Produtos",
                principalTable: "Ingrediente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoEstoque_Produto_ProdutoId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                column: "ProdutoId",
                principalSchema: "Produtos",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoEstoque_Usuario_UsuarioId",
                schema: "Estoque",
                table: "MovimentacaoEstoque",
                column: "UsuarioId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoFinanceira_Filial_FilialId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                column: "FilialId",
                principalSchema: "Filiais",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoFinanceira_Pedido_PedidoId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                column: "PedidoId",
                principalSchema: "Pedidos",
                principalTable: "Pedido",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoFinanceira_Usuario_UsuarioId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira",
                column: "UsuarioId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistroJornada_Funcionario_FuncionarioId",
                schema: "Funcionarios",
                table: "RegistroJornada",
                column: "FuncionarioId",
                principalSchema: "Funcionarios",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Core",
                table: "RoleClaim",
                column: "RoleId",
                principalSchema: "Core",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_Usuario_UserId",
                schema: "Core",
                table: "UserClaim",
                column: "UserId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_Usuario_UserId",
                schema: "Core",
                table: "UserLogin",
                column: "UserId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Core",
                table: "UserRole",
                column: "RoleId",
                principalSchema: "Core",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Usuario_UserId",
                schema: "Core",
                table: "UserRole",
                column: "UserId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToken_Usuario_UserId",
                schema: "Core",
                table: "UserToken",
                column: "UserId",
                principalSchema: "Core",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssinaturaEmpresa_Empresa_EmpresaId",
                schema: "Core",
                table: "AssinaturaEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_AssinaturaEmpresa_PlanoAssinatura_PlanoId",
                schema: "Core",
                table: "AssinaturaEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedido_Pedido_PedidoId",
                schema: "Pedidos",
                table: "ItemPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPedido_Produto_ProdutoId",
                schema: "Pedidos",
                table: "ItemPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_LogOperacao_Empresa_EmpresaId",
                schema: "Core",
                table: "LogOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogOperacao_Filial_FilialId",
                schema: "Core",
                table: "LogOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_LogOperacao_Usuario_UsuarioId",
                schema: "Core",
                table: "LogOperacao");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuloPlano_PlanoAssinatura_PlanoId",
                schema: "Core",
                table: "ModuloPlano");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoEstoque_Filial_FilialId",
                schema: "Estoque",
                table: "MovimentacaoEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoEstoque_Ingrediente_IngredienteId",
                schema: "Estoque",
                table: "MovimentacaoEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoEstoque_Produto_ProdutoId",
                schema: "Estoque",
                table: "MovimentacaoEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoEstoque_Usuario_UsuarioId",
                schema: "Estoque",
                table: "MovimentacaoEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoFinanceira_Filial_FilialId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoFinanceira_Pedido_PedidoId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoFinanceira_Usuario_UsuarioId",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistroJornada_Funcionario_FuncionarioId",
                schema: "Funcionarios",
                table: "RegistroJornada");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                schema: "Core",
                table: "RoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_Usuario_UserId",
                schema: "Core",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_Usuario_UserId",
                schema: "Core",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                schema: "Core",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Usuario_UserId",
                schema: "Core",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToken_Usuario_UserId",
                schema: "Core",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserToken",
                schema: "Core",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                schema: "Core",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogin",
                schema: "Core",
                table: "UserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaim",
                schema: "Core",
                table: "UserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaim",
                schema: "Core",
                table: "RoleClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                schema: "Core",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistroJornada",
                schema: "Funcionarios",
                table: "RegistroJornada");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanoAssinatura",
                schema: "Core",
                table: "PlanoAssinatura");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovimentacaoFinanceira",
                schema: "Financeiro",
                table: "MovimentacaoFinanceira");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovimentacaoEstoque",
                schema: "Estoque",
                table: "MovimentacaoEstoque");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuloPlano",
                schema: "Core",
                table: "ModuloPlano");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogOperacao",
                schema: "Core",
                table: "LogOperacao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemPedido",
                schema: "Pedidos",
                table: "ItemPedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssinaturaEmpresa",
                schema: "Core",
                table: "AssinaturaEmpresa");

            migrationBuilder.RenameTable(
                name: "UsuarioFilial",
                schema: "Filiais",
                newName: "UsuarioFilial");

            migrationBuilder.RenameTable(
                name: "Usuario",
                schema: "Core",
                newName: "Usuario");

            migrationBuilder.RenameTable(
                name: "SubAgrupamento",
                schema: "CentroCusto",
                newName: "SubAgrupamento");

            migrationBuilder.RenameTable(
                name: "ProdutoIngrediente",
                schema: "Produtos",
                newName: "ProdutoIngrediente");

            migrationBuilder.RenameTable(
                name: "Produto",
                schema: "Produtos",
                newName: "Produto");

            migrationBuilder.RenameTable(
                name: "Pedido",
                schema: "Pedidos",
                newName: "Pedido");

            migrationBuilder.RenameTable(
                name: "Mesa",
                schema: "Cardapio",
                newName: "Mesa");

            migrationBuilder.RenameTable(
                name: "Ingrediente",
                schema: "Produtos",
                newName: "Ingrediente");

            migrationBuilder.RenameTable(
                name: "Funcionario",
                schema: "Funcionarios",
                newName: "Funcionario");

            migrationBuilder.RenameTable(
                name: "FilialAgrupamento",
                schema: "CentroCusto",
                newName: "FilialAgrupamento");

            migrationBuilder.RenameTable(
                name: "Filial",
                schema: "Filiais",
                newName: "Filial");

            migrationBuilder.RenameTable(
                name: "Empresa",
                schema: "Empresas",
                newName: "Empresa");

            migrationBuilder.RenameTable(
                name: "CentroCusto",
                schema: "CentroCusto",
                newName: "CentroCusto");

            migrationBuilder.RenameTable(
                name: "Categoria",
                schema: "Categorias",
                newName: "Categoria");

            migrationBuilder.RenameTable(
                name: "Agrupamento",
                schema: "CentroCusto",
                newName: "Agrupamento");

            migrationBuilder.RenameTable(
                name: "UserToken",
                schema: "Core",
                newName: "AspNetUserToken");

            migrationBuilder.RenameTable(
                name: "UserRole",
                schema: "Core",
                newName: "AspNetUserRole");

            migrationBuilder.RenameTable(
                name: "UserLogin",
                schema: "Core",
                newName: "AspNetUserLogin");

            migrationBuilder.RenameTable(
                name: "UserClaim",
                schema: "Core",
                newName: "AspNetUserClaim");

            migrationBuilder.RenameTable(
                name: "RoleClaim",
                schema: "Core",
                newName: "AspNetRoleClaim");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "Core",
                newName: "AspNetRole");

            migrationBuilder.RenameTable(
                name: "RegistroJornada",
                schema: "Funcionarios",
                newName: "RegistrosJornada");

            migrationBuilder.RenameTable(
                name: "PlanoAssinatura",
                schema: "Core",
                newName: "PlanosAssinatura");

            migrationBuilder.RenameTable(
                name: "MovimentacaoFinanceira",
                schema: "Financeiro",
                newName: "MovimentacoesFinanceira");

            migrationBuilder.RenameTable(
                name: "MovimentacaoEstoque",
                schema: "Estoque",
                newName: "MovimentacoesEstoque");

            migrationBuilder.RenameTable(
                name: "ModuloPlano",
                schema: "Core",
                newName: "ModulosPlano");

            migrationBuilder.RenameTable(
                name: "LogOperacao",
                schema: "Core",
                newName: "LogsOperacao");

            migrationBuilder.RenameTable(
                name: "ItemPedido",
                schema: "Pedidos",
                newName: "ItensPedido");

            migrationBuilder.RenameTable(
                name: "AssinaturaEmpresa",
                schema: "Core",
                newName: "AssinaturasEmpresa");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "AspNetUserRole",
                newName: "IX_AspNetUserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogin_UserId",
                table: "AspNetUserLogin",
                newName: "IX_AspNetUserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaim_UserId",
                table: "AspNetUserClaim",
                newName: "IX_AspNetUserClaim_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaim_RoleId",
                table: "AspNetRoleClaim",
                newName: "IX_AspNetRoleClaim_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistroJornada_FuncionarioId",
                table: "RegistrosJornada",
                newName: "IX_RegistrosJornada_FuncionarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoFinanceira_UsuarioId",
                table: "MovimentacoesFinanceira",
                newName: "IX_MovimentacoesFinanceira_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoFinanceira_PedidoId",
                table: "MovimentacoesFinanceira",
                newName: "IX_MovimentacoesFinanceira_PedidoId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoFinanceira_FilialId",
                table: "MovimentacoesFinanceira",
                newName: "IX_MovimentacoesFinanceira_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoEstoque_UsuarioId",
                table: "MovimentacoesEstoque",
                newName: "IX_MovimentacoesEstoque_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                newName: "IX_MovimentacoesEstoque_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoEstoque_IngredienteId",
                table: "MovimentacoesEstoque",
                newName: "IX_MovimentacoesEstoque_IngredienteId");

            migrationBuilder.RenameIndex(
                name: "IX_MovimentacaoEstoque_FilialId",
                table: "MovimentacoesEstoque",
                newName: "IX_MovimentacoesEstoque_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuloPlano_PlanoId",
                table: "ModulosPlano",
                newName: "IX_ModulosPlano_PlanoId");

            migrationBuilder.RenameIndex(
                name: "IX_LogOperacao_UsuarioId",
                table: "LogsOperacao",
                newName: "IX_LogsOperacao_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_LogOperacao_FilialId",
                table: "LogsOperacao",
                newName: "IX_LogsOperacao_FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_LogOperacao_EmpresaId",
                table: "LogsOperacao",
                newName: "IX_LogsOperacao_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "ItensPedido",
                newName: "IX_ItensPedido_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemPedido_PedidoId",
                table: "ItensPedido",
                newName: "IX_ItensPedido_PedidoId");

            migrationBuilder.RenameIndex(
                name: "IX_AssinaturaEmpresa_PlanoId",
                table: "AssinaturasEmpresa",
                newName: "IX_AssinaturasEmpresa_PlanoId");

            migrationBuilder.RenameIndex(
                name: "IX_AssinaturaEmpresa_EmpresaId",
                table: "AssinaturasEmpresa",
                newName: "IX_AssinaturasEmpresa_EmpresaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserToken",
                table: "AspNetUserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRole",
                table: "AspNetUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogin",
                table: "AspNetUserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaim",
                table: "AspNetUserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaim",
                table: "AspNetRoleClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRole",
                table: "AspNetRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrosJornada",
                table: "RegistrosJornada",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanosAssinatura",
                table: "PlanosAssinatura",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovimentacoesFinanceira",
                table: "MovimentacoesFinanceira",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovimentacoesEstoque",
                table: "MovimentacoesEstoque",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModulosPlano",
                table: "ModulosPlano",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogsOperacao",
                table: "LogsOperacao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItensPedido",
                table: "ItensPedido",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssinaturasEmpresa",
                table: "AssinaturasEmpresa",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
                table: "AspNetRoleClaim",
                column: "RoleId",
                principalTable: "AspNetRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaim_Usuario_UserId",
                table: "AspNetUserClaim",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogin_Usuario_UserId",
                table: "AspNetUserLogin",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRole_AspNetRole_RoleId",
                table: "AspNetUserRole",
                column: "RoleId",
                principalTable: "AspNetRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRole_Usuario_UserId",
                table: "AspNetUserRole",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserToken_Usuario_UserId",
                table: "AspNetUserToken",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssinaturasEmpresa_Empresa_EmpresaId",
                table: "AssinaturasEmpresa",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssinaturasEmpresa_PlanosAssinatura_PlanoId",
                table: "AssinaturasEmpresa",
                column: "PlanoId",
                principalTable: "PlanosAssinatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPedido_Pedido_PedidoId",
                table: "ItensPedido",
                column: "PedidoId",
                principalTable: "Pedido",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensPedido_Produto_ProdutoId",
                table: "ItensPedido",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsOperacao_Empresa_EmpresaId",
                table: "LogsOperacao",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsOperacao_Filial_FilialId",
                table: "LogsOperacao",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsOperacao_Usuario_UsuarioId",
                table: "LogsOperacao",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModulosPlano_PlanosAssinatura_PlanoId",
                table: "ModulosPlano",
                column: "PlanoId",
                principalTable: "PlanosAssinatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesEstoque_Filial_FilialId",
                table: "MovimentacoesEstoque",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesEstoque_Ingrediente_IngredienteId",
                table: "MovimentacoesEstoque",
                column: "IngredienteId",
                principalTable: "Ingrediente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesEstoque_Produto_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesEstoque_Usuario_UsuarioId",
                table: "MovimentacoesEstoque",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesFinanceira_Filial_FilialId",
                table: "MovimentacoesFinanceira",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesFinanceira_Pedido_PedidoId",
                table: "MovimentacoesFinanceira",
                column: "PedidoId",
                principalTable: "Pedido",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacoesFinanceira_Usuario_UsuarioId",
                table: "MovimentacoesFinanceira",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosJornada_Funcionario_FuncionarioId",
                table: "RegistrosJornada",
                column: "FuncionarioId",
                principalTable: "Funcionario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

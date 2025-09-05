using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoRestaurante.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFilialAgrupamentoAndProdutoIngrediente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agrupamento_Empresa_EmpresaId",
                table: "Agrupamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Filial_FilialId",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioFilial_Usuario_UsuarioId1",
                table: "UsuarioFilial");

            migrationBuilder.DropTable(
                name: "FilialCentroCusto");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioFilial_UsuarioId1",
                table: "UsuarioFilial");

            migrationBuilder.DropIndex(
                name: "IX_Filial_Cnpj",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "UsuarioFilial");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "SubAgrupamento");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "CentroCusto");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Categoria");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Agrupamento");

            migrationBuilder.RenameColumn(
                name: "Cnpj",
                table: "Filial",
                newName: "CnpjFilial");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Agrupamento",
                newName: "FilialId");

            migrationBuilder.RenameIndex(
                name: "IX_Agrupamento_EmpresaId_Codigo",
                table: "Agrupamento",
                newName: "IX_Agrupamento_FilialId_Codigo");

            migrationBuilder.AlterColumn<Guid>(
                name: "FilialId",
                table: "Usuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "SubAgrupamento",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Produto",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Bairro",
                table: "Filial",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cep",
                table: "Filial",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cidade",
                table: "Filial",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Complemento",
                table: "Filial",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Estado",
                table: "Filial",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Logradouro",
                table: "Filial",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Numero",
                table: "Filial",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Matriz",
                table: "Filial",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Bairro",
                table: "Empresa",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cep",
                table: "Empresa",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cidade",
                table: "Empresa",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Complemento",
                table: "Empresa",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Estado",
                table: "Empresa",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Logradouro",
                table: "Empresa",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Numero",
                table: "Empresa",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "CentroCusto",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Categoria",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Agrupamento",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "FilialAgrupamento",
                columns: table => new
                {
                    FilialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgrupamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataVinculo = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilialAgrupamento", x => new { x.FilialId, x.AgrupamentoId });
                    table.ForeignKey(
                        name: "FK_FilialAgrupamento_Agrupamento_AgrupamentoId",
                        column: x => x.AgrupamentoId,
                        principalTable: "Agrupamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilialAgrupamento_Filial_FilialId",
                        column: x => x.FilialId,
                        principalTable: "Filial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Filial_CnpjFilial",
                table: "Filial",
                column: "CnpjFilial",
                unique: true,
                filter: "CnpjFilial IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Filial_EmpresaId_Matriz_Unica",
                table: "Filial",
                columns: new[] { "EmpresaId", "Matriz" },
                unique: true,
                filter: "Matriz = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FilialAgrupamento_AgrupamentoId",
                table: "FilialAgrupamento",
                column: "AgrupamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_FilialAgrupamentos_Ativo",
                table: "FilialAgrupamento",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_FilialAgrupamentos_DataVinculo",
                table: "FilialAgrupamento",
                column: "DataVinculo");

            migrationBuilder.AddForeignKey(
                name: "FK_Agrupamento_Filial_FilialId",
                table: "Agrupamento",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Filial_FilialId",
                table: "Usuario",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agrupamento_Filial_FilialId",
                table: "Agrupamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Filial_FilialId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "FilialAgrupamento");

            migrationBuilder.DropIndex(
                name: "IX_Filial_CnpjFilial",
                table: "Filial");

            migrationBuilder.DropIndex(
                name: "IX_Filial_EmpresaId_Matriz_Unica",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Bairro",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Cep",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Cidade",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Complemento",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Estado",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Logradouro",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Numero",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Matriz",
                table: "Filial");

            migrationBuilder.DropColumn(
                name: "Endereco_Bairro",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Cep",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Cidade",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Complemento",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Estado",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Logradouro",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "Endereco_Numero",
                table: "Empresa");

            migrationBuilder.RenameColumn(
                name: "CnpjFilial",
                table: "Filial",
                newName: "Cnpj");

            migrationBuilder.RenameColumn(
                name: "FilialId",
                table: "Agrupamento",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Agrupamento_FilialId_Codigo",
                table: "Agrupamento",
                newName: "IX_Agrupamento_EmpresaId_Codigo");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId1",
                table: "UsuarioFilial",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FilialId",
                table: "Usuario",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "SubAgrupamento",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "SubAgrupamento",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Produto",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Produto",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Filial",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Empresa",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "CentroCusto",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "CentroCusto",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Categoria",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Categoria",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativa",
                table: "Agrupamento",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Agrupamento",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "FilialCentroCusto",
                columns: table => new
                {
                    FilialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CentroCustoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CentroCustoId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataDesvinculo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataVinculo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilialId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilialCentroCusto", x => new { x.FilialId, x.CentroCustoId });
                    table.ForeignKey(
                        name: "FK_FilialCentroCusto_CentroCusto_CentroCustoId",
                        column: x => x.CentroCustoId,
                        principalTable: "CentroCusto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilialCentroCusto_CentroCusto_CentroCustoId1",
                        column: x => x.CentroCustoId1,
                        principalTable: "CentroCusto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FilialCentroCusto_Filial_FilialId",
                        column: x => x.FilialId,
                        principalTable: "Filial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilialCentroCusto_Filial_FilialId1",
                        column: x => x.FilialId1,
                        principalTable: "Filial",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioFilial_UsuarioId1",
                table: "UsuarioFilial",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Filial_Cnpj",
                table: "Filial",
                column: "Cnpj",
                unique: true,
                filter: "Cnpj IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FilialCentroCusto_CentroCustoId",
                table: "FilialCentroCusto",
                column: "CentroCustoId");

            migrationBuilder.CreateIndex(
                name: "IX_FilialCentroCusto_CentroCustoId1",
                table: "FilialCentroCusto",
                column: "CentroCustoId1");

            migrationBuilder.CreateIndex(
                name: "IX_FilialCentroCusto_FilialId1",
                table: "FilialCentroCusto",
                column: "FilialId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Agrupamento_Empresa_EmpresaId",
                table: "Agrupamento",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Filial_FilialId",
                table: "Usuario",
                column: "FilialId",
                principalTable: "Filial",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioFilial_Usuario_UsuarioId1",
                table: "UsuarioFilial",
                column: "UsuarioId1",
                principalTable: "Usuario",
                principalColumn: "Id");
        }
    }
}

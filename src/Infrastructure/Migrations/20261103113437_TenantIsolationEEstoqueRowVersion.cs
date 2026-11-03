using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenantIsolationEEstoqueRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Produtos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "MovimentacaoEstoques",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Jogos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ItensCompras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Fornecedores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Estoques",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Estoques",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Compras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Colecoes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Categorias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Dados existentes eram compartilhados por todos os usuários: atribui-os ao tenant do primeiro
            // usuário cadastrado para que não fiquem invisíveis após o isolamento por tenant.
            migrationBuilder.Sql(@"
DECLARE @tenant uniqueidentifier = (SELECT TOP 1 TenantId FROM AspNetUsers WHERE TenantId <> '00000000-0000-0000-0000-000000000000' ORDER BY Email);
IF @tenant IS NOT NULL
BEGIN
    UPDATE Categorias SET TenantId = @tenant;
    UPDATE Colecoes SET TenantId = @tenant;
    UPDATE Compras SET TenantId = @tenant;
    UPDATE ItensCompras SET TenantId = @tenant;
    UPDATE Estoques SET TenantId = @tenant;
    UPDATE MovimentacaoEstoques SET TenantId = @tenant;
    UPDATE Fornecedores SET TenantId = @tenant;
    UPDATE Jogos SET TenantId = @tenant;
    UPDATE Produtos SET TenantId = @tenant;
END");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_TenantId",
                table: "Produtos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacaoEstoques_TenantId",
                table: "MovimentacaoEstoques",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_TenantId",
                table: "Jogos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompras_TenantId",
                table: "ItensCompras",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_TenantId",
                table: "Fornecedores",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_TenantId",
                table: "Estoques",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_TenantId",
                table: "Compras",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Colecoes_TenantId",
                table: "Colecoes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_TenantId",
                table: "Categorias",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Produtos_TenantId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_MovimentacaoEstoques_TenantId",
                table: "MovimentacaoEstoques");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_TenantId",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_ItensCompras_TenantId",
                table: "ItensCompras");

            migrationBuilder.DropIndex(
                name: "IX_Fornecedores_TenantId",
                table: "Fornecedores");

            migrationBuilder.DropIndex(
                name: "IX_Estoques_TenantId",
                table: "Estoques");

            migrationBuilder.DropIndex(
                name: "IX_Compras_TenantId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Colecoes_TenantId",
                table: "Colecoes");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_TenantId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MovimentacaoEstoques");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ItensCompras");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Fornecedores");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Estoques");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Estoques");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Colecoes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Categorias");
        }
    }
}

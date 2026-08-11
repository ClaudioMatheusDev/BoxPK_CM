using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustaRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDColecao",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IDCategoria",
                table: "Produtos",
                column: "IDCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IDColecao",
                table: "Produtos",
                column: "IDColecao");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IDJogoTCG",
                table: "Produtos",
                column: "IDJogoTCG");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacaoEstoques_IDProduto",
                table: "MovimentacaoEstoques",
                column: "IDProduto");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompras_IDCompra",
                table: "ItensCompras",
                column: "IDCompra");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompras_IDProduto",
                table: "ItensCompras",
                column: "IDProduto");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_IDProduto",
                table: "Estoques",
                column: "IDProduto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_IDFornecedor",
                table: "Compras",
                column: "IDFornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_Colecoes_IDJogoTCG",
                table: "Colecoes",
                column: "IDJogoTCG");

            migrationBuilder.AddForeignKey(
                name: "FK_Colecoes_Jogos_IDJogoTCG",
                table: "Colecoes",
                column: "IDJogoTCG",
                principalTable: "Jogos",
                principalColumn: "IDJogoTCG",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Fornecedores_IDFornecedor",
                table: "Compras",
                column: "IDFornecedor",
                principalTable: "Fornecedores",
                principalColumn: "IDFornecedor",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Estoques_Produtos_IDProduto",
                table: "Estoques",
                column: "IDProduto",
                principalTable: "Produtos",
                principalColumn: "IDProduto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensCompras_Compras_IDCompra",
                table: "ItensCompras",
                column: "IDCompra",
                principalTable: "Compras",
                principalColumn: "IDCompra",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensCompras_Produtos_IDProduto",
                table: "ItensCompras",
                column: "IDProduto",
                principalTable: "Produtos",
                principalColumn: "IDProduto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentacaoEstoques_Produtos_IDProduto",
                table: "MovimentacaoEstoques",
                column: "IDProduto",
                principalTable: "Produtos",
                principalColumn: "IDProduto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_IDCategoria",
                table: "Produtos",
                column: "IDCategoria",
                principalTable: "Categorias",
                principalColumn: "IDCategoria",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Colecoes_IDColecao",
                table: "Produtos",
                column: "IDColecao",
                principalTable: "Colecoes",
                principalColumn: "IDColecao",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Jogos_IDJogoTCG",
                table: "Produtos",
                column: "IDJogoTCG",
                principalTable: "Jogos",
                principalColumn: "IDJogoTCG",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colecoes_Jogos_IDJogoTCG",
                table: "Colecoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Fornecedores_IDFornecedor",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK_Estoques_Produtos_IDProduto",
                table: "Estoques");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensCompras_Compras_IDCompra",
                table: "ItensCompras");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensCompras_Produtos_IDProduto",
                table: "ItensCompras");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimentacaoEstoques_Produtos_IDProduto",
                table: "MovimentacaoEstoques");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_IDCategoria",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Colecoes_IDColecao",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Jogos_IDJogoTCG",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_IDCategoria",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_IDColecao",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_IDJogoTCG",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_MovimentacaoEstoques_IDProduto",
                table: "MovimentacaoEstoques");

            migrationBuilder.DropIndex(
                name: "IX_ItensCompras_IDCompra",
                table: "ItensCompras");

            migrationBuilder.DropIndex(
                name: "IX_ItensCompras_IDProduto",
                table: "ItensCompras");

            migrationBuilder.DropIndex(
                name: "IX_Estoques_IDProduto",
                table: "Estoques");

            migrationBuilder.DropIndex(
                name: "IX_Compras_IDFornecedor",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Colecoes_IDJogoTCG",
                table: "Colecoes");

            migrationBuilder.DropColumn(
                name: "IDColecao",
                table: "Produtos");
        }
    }
}

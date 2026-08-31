using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Audit
{
    /// <inheritdoc />
    public partial class InitialAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    IDAuditEntry = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDTabelaAfetada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeTabelaAfetada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNovos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Acao = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomeUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.IDAuditEntry);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEntries");
        }
    }
}

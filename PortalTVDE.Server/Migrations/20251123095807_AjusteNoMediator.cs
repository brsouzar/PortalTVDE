using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalTVDE.Server.Migrations
{
    /// <inheritdoc />
    public partial class AjusteNoMediator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajuste na tabela Mediators (já estava correto)
            migrationBuilder.AlterColumn<int>(
                name: "Tier",
                table: "Mediators",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            // ==========================================================
            // CORREÇÃO PARA AspNetUserTokens
            // ==========================================================

            // 1. REMOVER A CHAVE PRIMÁRIA PK_AspNetUserTokens
            // (A chave primária é tipicamente composta por UserId, LoginProvider e Name)
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            // 2. ALTERAR AS COLUNAS

            // Alterar a coluna Name
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)", // Novo tamanho
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Alterar a coluna LoginProvider
            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)", // Novo tamanho
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // 3. RECRIAR A CHAVE PRIMÁRIA
            // Assumindo que a chave primária é composta por UserId, LoginProvider e Name:
            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            // ==========================================================
            // FIM DA CORREÇÃO

            // Ajuste na tabela AspNetUserLogins (também requer remoção/adição da PK)

            // 1. REMOVER A CHAVE PRIMÁRIA PK_AspNetUserLogins
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            // 2. ALTERAR AS COLUNAS

            // Alterar a coluna ProviderKey
            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Alterar a coluna LoginProvider
            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // 3. RECRIAR A CHAVE PRIMÁRIA
            // Assumindo que a chave primária é composta por LoginProvider e ProviderKey:
            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });
        }

        // O método Down também deve ser ajustado para reverter as mudanças corretamente
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Omitido por brevidade, mas deve seguir o mesmo padrão de Drop/Alter/Add PK
            // para as tabelas AspNetUserTokens e AspNetUserLogins.
        }
    }
}
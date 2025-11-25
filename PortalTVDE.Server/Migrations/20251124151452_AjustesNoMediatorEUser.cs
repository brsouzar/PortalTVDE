using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalTVDE.Server.Migrations
{
    /// <inheritdoc />
    public partial class AjustesNoMediatorEUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Mediators_MediatorId",
                table: "Quotes");

            migrationBuilder.AlterColumn<int>(
                name: "MediatorId",
                table: "Quotes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MediatorId",
                table: "AspNetUsers",
                column: "MediatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Mediators_MediatorId",
                table: "AspNetUsers",
                column: "MediatorId",
                principalTable: "Mediators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Mediators_MediatorId",
                table: "Quotes",
                column: "MediatorId",
                principalTable: "Mediators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Mediators_MediatorId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Mediators_MediatorId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MediatorId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "MediatorId",
                table: "Quotes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Mediators_MediatorId",
                table: "Quotes",
                column: "MediatorId",
                principalTable: "Mediators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

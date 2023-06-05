using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasApi.Migrations
{
    /// <inheritdoc />
    public partial class DataPrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeliculasGeneros_Peliculas_peliculaId",
                table: "PeliculasGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros");

            migrationBuilder.DropColumn(
                name: "PeliculasId",
                table: "PeliculasGeneros");

            migrationBuilder.RenameColumn(
                name: "peliculaId",
                table: "PeliculasGeneros",
                newName: "PeliculaId");

            migrationBuilder.RenameIndex(
                name: "IX_PeliculasGeneros_peliculaId",
                table: "PeliculasGeneros",
                newName: "IX_PeliculasGeneros_PeliculaId");

            migrationBuilder.AlterColumn<int>(
                name: "PeliculaId",
                table: "PeliculasGeneros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros",
                columns: new[] { "GeneroId", "PeliculaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PeliculasGeneros_Peliculas_PeliculaId",
                table: "PeliculasGeneros",
                column: "PeliculaId",
                principalTable: "Peliculas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeliculasGeneros_Peliculas_PeliculaId",
                table: "PeliculasGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros");

            migrationBuilder.RenameColumn(
                name: "PeliculaId",
                table: "PeliculasGeneros",
                newName: "peliculaId");

            migrationBuilder.RenameIndex(
                name: "IX_PeliculasGeneros_PeliculaId",
                table: "PeliculasGeneros",
                newName: "IX_PeliculasGeneros_peliculaId");

            migrationBuilder.AlterColumn<int>(
                name: "peliculaId",
                table: "PeliculasGeneros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PeliculasId",
                table: "PeliculasGeneros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros",
                columns: new[] { "GeneroId", "PeliculasId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PeliculasGeneros_Peliculas_peliculaId",
                table: "PeliculasGeneros",
                column: "peliculaId",
                principalTable: "Peliculas",
                principalColumn: "Id");
        }
    }
}

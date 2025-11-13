using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aplicacion.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClaseActivaFKToAsistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Materia",
                table: "Asistencia");

            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "Asistencia",
                newName: "FechaRegistro");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Asistencia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ClaseActivaId",
                table: "Asistencia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_ClaseActivaId",
                table: "Asistencia",
                column: "ClaseActivaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_ClasesActivas_ClaseActivaId",
                table: "Asistencia",
                column: "ClaseActivaId",
                principalTable: "ClasesActivas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_ClasesActivas_ClaseActivaId",
                table: "Asistencia");

            migrationBuilder.DropIndex(
                name: "IX_Asistencia_ClaseActivaId",
                table: "Asistencia");

            migrationBuilder.DropColumn(
                name: "ClaseActivaId",
                table: "Asistencia");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "Asistencia",
                newName: "FechaHora");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Asistencia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Materia",
                table: "Asistencia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aplicacion.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationToAlumnoDocente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Docente",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Alumno",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docente_ApplicationUserId",
                table: "Docente",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alumno_ApplicationUserId",
                table: "Alumno",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alumno_AspNetUsers_ApplicationUserId",
                table: "Alumno",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Docente_AspNetUsers_ApplicationUserId",
                table: "Docente",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alumno_AspNetUsers_ApplicationUserId",
                table: "Alumno");

            migrationBuilder.DropForeignKey(
                name: "FK_Docente_AspNetUsers_ApplicationUserId",
                table: "Docente");

            migrationBuilder.DropIndex(
                name: "IX_Docente_ApplicationUserId",
                table: "Docente");

            migrationBuilder.DropIndex(
                name: "IX_Alumno_ApplicationUserId",
                table: "Alumno");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Docente");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Alumno");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aplicacion.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrecionIdAsistencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DocenteId",
                table: "ClasesActivas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocenteId",
                table: "ClasesActivas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}

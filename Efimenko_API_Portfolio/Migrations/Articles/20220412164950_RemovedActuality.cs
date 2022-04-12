using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efimenko_API_Portfolio.Migrations.Articles
{
    public partial class RemovedActuality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actuality",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "CreationDate",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "Actuality",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

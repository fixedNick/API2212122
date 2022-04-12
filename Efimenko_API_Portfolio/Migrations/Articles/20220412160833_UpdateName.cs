using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Efimenko_API_Portfolio.Migrations.Articles
{
    public partial class UpdateName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValueLevel",
                table: "Articles",
                newName: "Actuality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Actuality",
                table: "Articles",
                newName: "ValueLevel");
        }
    }
}

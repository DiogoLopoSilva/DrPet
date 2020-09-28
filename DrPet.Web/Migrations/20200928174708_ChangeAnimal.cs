using Microsoft.EntityFrameworkCore.Migrations;

namespace DrPet.Web.Migrations
{
    public partial class ChangeAnimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Animals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Animals",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}

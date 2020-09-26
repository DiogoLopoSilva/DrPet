using Microsoft.EntityFrameworkCore.Migrations;

namespace DrPet.Web.Migrations
{
    public partial class UserSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Admins",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Admins");
        }
    }
}

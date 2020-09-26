using Microsoft.EntityFrameworkCore.Migrations;

namespace DrPet.Web.Migrations
{
    public partial class ChangeIsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "AspNetUsers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Admins",
                newName: "IsDeleted");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Specializations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Doctors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Appointments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Animals",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Admins",
                newName: "isDeleted");
        }
    }
}

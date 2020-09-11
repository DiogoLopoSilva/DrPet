using Microsoft.EntityFrameworkCore.Migrations;

namespace DrPet.Web.Migrations
{
    public partial class ChangeAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Appointments",
                newName: "DoctorNotes");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientDescription",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientDescription",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "DoctorNotes",
                table: "Appointments",
                newName: "Notes");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

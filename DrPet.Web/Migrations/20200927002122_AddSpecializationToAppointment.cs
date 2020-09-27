using Microsoft.EntityFrameworkCore.Migrations;

namespace DrPet.Web.Migrations
{
    public partial class AddSpecializationToAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "Appointments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SpecializationId",
                table: "Appointments",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Specializations_SpecializationId",
                table: "Appointments",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Specializations_SpecializationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SpecializationId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Appointments",
                nullable: false,
                defaultValue: "");
        }
    }
}

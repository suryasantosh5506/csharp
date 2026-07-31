using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorApplicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "DoctorApplications",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "DoctorApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "DoctorApplications");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "DoctorApplications");
        }
    }
}

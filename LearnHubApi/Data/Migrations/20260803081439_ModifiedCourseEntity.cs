using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnHubApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedCourseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Courses");
        }
    }
}

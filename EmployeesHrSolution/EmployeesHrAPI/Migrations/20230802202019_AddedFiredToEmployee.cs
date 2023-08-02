using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesHrAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedFiredToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Fired",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fired",
                table: "Employees");
        }
    }
}

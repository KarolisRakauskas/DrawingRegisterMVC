using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrawingRegisterWeb.Migrations
{
    /// <inheritdoc />
    public partial class ModelUrlToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelUrl",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelUrl",
                table: "Project");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBox.Advanced.Migrations
{
    /// <inheritdoc />
    public partial class v43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRestored",
                table: "Events",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRestored",
                table: "Events");
        }
    }
}

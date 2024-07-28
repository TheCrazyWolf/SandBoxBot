using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBox.Advanced.Migrations
{
    /// <inheritdoc />
    public partial class v42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoKickIfWillBeDetectedSpam",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoKickIfWillBeDetectedSpam",
                table: "Chats");
        }
    }
}

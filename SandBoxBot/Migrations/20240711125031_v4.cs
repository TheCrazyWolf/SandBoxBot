using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBoxBot.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Accounts_IdAccountTelegram",
                table: "Sentences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sentences",
                table: "Sentences");

            migrationBuilder.RenameTable(
                name: "Sentences",
                newName: "Incidents");

            migrationBuilder.RenameIndex(
                name: "IX_Sentences_IdAccountTelegram",
                table: "Incidents",
                newName: "IX_Incidents_IdAccountTelegram");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incidents",
                table: "Incidents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Accounts_IdAccountTelegram",
                table: "Incidents",
                column: "IdAccountTelegram",
                principalTable: "Accounts",
                principalColumn: "IdAccountTelegram",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Accounts_IdAccountTelegram",
                table: "Incidents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incidents",
                table: "Incidents");

            migrationBuilder.RenameTable(
                name: "Incidents",
                newName: "Sentences");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_IdAccountTelegram",
                table: "Sentences",
                newName: "IX_Sentences_IdAccountTelegram");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sentences",
                table: "Sentences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Accounts_IdAccountTelegram",
                table: "Sentences",
                column: "IdAccountTelegram",
                principalTable: "Accounts",
                principalColumn: "IdAccountTelegram");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBoxBot.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Sentences",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "IdAccountTelegram",
                table: "Sentences",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    IdAccountTelegram = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    DateTimeJoined = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.IdAccountTelegram);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_IdAccountTelegram",
                table: "Sentences",
                column: "IdAccountTelegram");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Accounts_IdAccountTelegram",
                table: "Sentences",
                column: "IdAccountTelegram",
                principalTable: "Accounts",
                principalColumn: "IdAccountTelegram");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Accounts_IdAccountTelegram",
                table: "Sentences");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Sentences_IdAccountTelegram",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "IdAccountTelegram",
                table: "Sentences");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBox.Advanced.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chats",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<long>(
                name: "CountNormalMessageToBeAprroved",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Chats",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Chats",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PercentageToDetectSpamFromMl",
                table: "Chats",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Chats",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MembersInChats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdTelegram = table.Column<long>(type: "INTEGER", nullable: true),
                    IdChat = table.Column<long>(type: "INTEGER", nullable: true),
                    DateTimeJoined = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRestricted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CountSpam = table.Column<long>(type: "INTEGER", nullable: false),
                    CountMessage = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersInChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembersInChats_Accounts_IdTelegram",
                        column: x => x.IdTelegram,
                        principalTable: "Accounts",
                        principalColumn: "IdTelegram");
                    table.ForeignKey(
                        name: "FK_MembersInChats_Chats_IdChat",
                        column: x => x.IdChat,
                        principalTable: "Chats",
                        principalColumn: "IdChat");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembersInChats_IdChat",
                table: "MembersInChats",
                column: "IdChat");

            migrationBuilder.CreateIndex(
                name: "IX_MembersInChats_IdTelegram",
                table: "MembersInChats",
                column: "IdTelegram");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembersInChats");

            migrationBuilder.DropColumn(
                name: "CountNormalMessageToBeAprroved",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "PercentageToDetectSpamFromMl",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Chats",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}

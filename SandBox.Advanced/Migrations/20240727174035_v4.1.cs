using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandBox.Advanced.Migrations
{
    /// <inheritdoc />
    public partial class v41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackWords");

            migrationBuilder.DropColumn(
                name: "CountMessage",
                table: "MembersInChats");

            migrationBuilder.DropColumn(
                name: "CountSpam",
                table: "MembersInChats");

            migrationBuilder.DropColumn(
                name: "CountNormalMessageToBeAprroved",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "DateTimeJoined",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsAprroved",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "IsSpamer",
                table: "Accounts",
                newName: "IsGlobalRestricted");

            migrationBuilder.RenameColumn(
                name: "IsNeedToVerifyByCaptcha",
                table: "Accounts",
                newName: "IsGlobalApproved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsGlobalRestricted",
                table: "Accounts",
                newName: "IsSpamer");

            migrationBuilder.RenameColumn(
                name: "IsGlobalApproved",
                table: "Accounts",
                newName: "IsNeedToVerifyByCaptcha");

            migrationBuilder.AddColumn<long>(
                name: "CountMessage",
                table: "MembersInChats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CountSpam",
                table: "MembersInChats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CountNormalMessageToBeAprroved",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeJoined",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAprroved",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BlackWords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackWords", x => x.Id);
                });
        }
    }
}

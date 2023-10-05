using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.DataAccess.Migrations
{
    public partial class AddRefreshTokenMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenCreated",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpires",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenCreated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokenExpires",
                table: "Users");
        }
    }
}

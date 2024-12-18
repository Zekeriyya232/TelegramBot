using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class TaskMemberAndTaskUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "creationTime",
                table: "Tasks",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "progress",
                table: "taskMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "progressPercent",
                table: "taskMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creationTime",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "progress",
                table: "taskMembers");

            migrationBuilder.DropColumn(
                name: "progressPercent",
                table: "taskMembers");
        }
    }
}

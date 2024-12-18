using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class RelationUserMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "memberId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "projectStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_memberId",
                table: "Users",
                column: "memberId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Members_memberId",
                table: "Users",
                column: "memberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Members_memberId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "projectStatus");

            migrationBuilder.DropIndex(
                name: "IX_Users_memberId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "memberId",
                table: "Users");
        }
    }
}

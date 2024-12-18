using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelegramBot.Migrations
{
    public partial class NullColumnUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
              migrationBuilder.CreateTable(
                 name: "ChatMember",
                 columns: table => new
                 {
                     Id = table.Column<long>(type: "bigint", nullable: false)
                         .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                     userName = table.Column<string>(type: "text", nullable: true),
                     telegramId = table.Column<long>(type: "bigint", nullable: false),
                     firstName = table.Column<string>(type: "text", nullable: true),
                     lastName = table.Column<string>(type: "text", nullable: true)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_ChatMember", x => x.Id);
                 });

             migrationBuilder.CreateTable(
                 name: "Users",
                 columns: table => new
                 {
                     Id = table.Column<int>(type: "integer", nullable: false)
                         .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                     userName = table.Column<string>(type: "text", nullable: false),
                     userSurname = table.Column<string>(type: "text", nullable: false),
                     userEmail = table.Column<string>(type: "text", nullable: false),
                     password = table.Column<string>(type: "text", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Users", x => x.Id);
                 });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            
            migrationBuilder.DropTable(
                name: "ChatMember");

            migrationBuilder.DropTable(
                name: "Users");

          
        }
    }
}

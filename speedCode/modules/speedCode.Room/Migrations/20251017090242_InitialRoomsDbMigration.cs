using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace speedCode.Room.Migrations
{
    /// <inheritdoc />
    public partial class InitialRoomsDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roomName = table.Column<string>(type: "text", nullable: false),
                    roomLevel = table.Column<string>(type: "text", nullable: false),
                    isPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    chatId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}

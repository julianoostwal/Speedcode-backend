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
                    chatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_rooms_profiles_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    exerciseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_exercises_exercises_exerciseId",
                        column: x => x.exerciseId,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_room_exercises_rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    profileId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_users_profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_room_users_rooms_roomId",
                        column: x => x.roomId,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_room_exercises_exerciseId",
                table: "room_exercises",
                column: "exerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_room_exercises_roomId",
                table: "room_exercises",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_room_users_profileId",
                table: "room_users",
                column: "profileId");

            migrationBuilder.CreateIndex(
                name: "IX_room_users_roomId",
                table: "room_users",
                column: "roomId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_CreatedById",
                table: "rooms",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_exercises");

            migrationBuilder.DropTable(
                name: "room_users");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
